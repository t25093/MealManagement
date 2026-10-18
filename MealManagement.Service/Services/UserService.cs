using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace MealManagement.Service.Services
{
    public class UserService : IUserService
    {
        private const int MaxPageSize = 50;
        private static readonly TimeSpan PasswordResetLifetime = TimeSpan.FromHours(1);

        private readonly IUserRepository _users;
        private readonly IMealInvitationRepository _invitations;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IEmailNotifications _emails;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository users,
            IMealInvitationRepository invitations,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IEmailNotifications emails,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _users = users;
            _invitations = invitations;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _emails = emails;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
        {
            var email = NormalizeEmail(dto.Email);

            if (await _users.EmailExistsAsync(email, ct))
                throw new ConflictException("כתובת האימייל כבר רשומה במערכת");

            var user = new User
            {
                FullName = dto.FullName.Trim(),
                Email = email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = UserRole.User,
                CreateAt = DateTime.UtcNow
            };

            await _users.AddAsync(user, ct);

            // הזמנות שחיכו לאימייל הזה הופכות להשתתפות בארוחות
            var invitations = await _invitations.GetByEmailAsync(email, ct);
            foreach (var invitation in invitations)
                user.ParticipatingMeals.Add(invitation.Meal);
            _invitations.RemoveRange(invitations);

            // שמירה אחת: המשתמשת, ההשתתפויות ומחיקת ההזמנות יחד
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} registered, joined {Count} invited meals", user.Id, invitations.Count);

            return _tokenService.CreateToken(_mapper.Map<UserDto>(user));
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
        {
            var email = NormalizeEmail(dto.Email);
            var user = await _users.GetByEmailAsync(email, ct);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt");
                throw new UnauthorizedException("אימייל או סיסמה שגויים");
            }

            _logger.LogInformation("User {UserId} logged in", user.Id);

            return _tokenService.CreateToken(_mapper.Map<UserDto>(user));
        }

        public async Task<UserDto> GetByIdAsync(int userId, CancellationToken ct = default)
        {
            var user = await _users.GetByIdAsync(userId, ct)
                ?? throw new NotFoundException("המשתמשת לא נמצאה");

            return _mapper.Map<UserDto>(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto, CancellationToken ct = default)
        {
            var found = await _users.GetByEmailAsync(NormalizeEmail(dto.Email), ct);

            // אימייל שלא רשום: מסיימים בשקט, כדי לא לגלות מי רשומה במערכת
            if (found == null)
            {
                _logger.LogInformation("Password reset requested for an unknown email");
                return;
            }

            // טעינה מחדש עם מעקב, לשמירת הקוד
            var user = await _users.GetByIdAsync(found.Id, ct)
                ?? throw new NotFoundException("המשתמשת לא נמצאה");

            // הקוד עצמו נשלח במייל, ובבסיס הנתונים נשמר רק ה-hash שלו
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .TrimEnd('=').Replace('+', '-').Replace('/', '_');
            user.PasswordResetTokenHash = HashToken(token);
            user.PasswordResetExpiresAt = DateTime.UtcNow.Add(PasswordResetLifetime);
            await _unitOfWork.SaveChangesAsync(ct);

            await _emails.SendPasswordResetAsync(user, token, ct);

            _logger.LogInformation("Password reset email sent to user {UserId}", user.Id);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken ct = default)
        {
            var user = await _users.GetByPasswordResetTokenHashAsync(HashToken(dto.Token), ct);

            if (user == null || user.PasswordResetExpiresAt == null || user.PasswordResetExpiresAt < DateTime.UtcNow)
                throw new BusinessRuleException("הקישור לאיפוס הסיסמה לא תקין או שפג תוקפו. אפשר לבקש קישור חדש");

            // הקוד משמש פעם אחת בלבד
            user.PasswordHash = _passwordHasher.Hash(dto.NewPassword);
            user.PasswordResetTokenHash = null;
            user.PasswordResetExpiresAt = null;
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("Password reset for user {UserId}", user.Id);
        }

        public async Task<PagedResult<UserDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

            var result = await _users.GetPagedAsync(page, pageSize, ct);

            return new PagedResult<UserDto>
            {
                Items = _mapper.Map<List<UserDto>>(result.Items),
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task DeleteAsync(int userId, int currentUserId, bool isAdmin, CancellationToken ct = default)
        {
            if (userId != currentUserId && !isAdmin)
                throw new ForbiddenException("אפשר למחוק רק את החשבון שלך");

            var user = await _users.GetForDeleteAsync(userId, ct)
                ?? throw new NotFoundException("המשתמשת לא נמצאה");

            // המנהלת היא הבעלים של מאגר המאכלים, ולכן החשבון שלה לא נמחק
            if (user.Role == UserRole.Admin)
                throw new ConflictException("לא ניתן למחוק חשבון של מנהלת מערכת");

            var adminId = await _users.GetAnyAdminIdAsync(ct)
                ?? throw new ConflictException("אין במערכת מנהלת שאליה יעברו המאכלים של המשתמשת");

            // מנות שהמשתמשת לקחה להכין חוזרות להיות פנויות לאחרות (כמו בביטול לקיחה)
            foreach (var assignment in user.Assignments)
                assignment.MealDish.AssignedQuantity =
                    Math.Max(0, assignment.MealDish.AssignedQuantity - assignment.Quantity);

            await _users.RemoveWithRelatedDataAsync(user, adminId, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            _logger.LogInformation("User {UserId} deleted by user {CurrentUserId}", userId, currentUserId);
        }

        private static string NormalizeEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        private static string HashToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }
    }
}