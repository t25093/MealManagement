using AutoMapper;
using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using MealManagement.Core.Services;
using Microsoft.Extensions.Logging;

namespace MealManagement.Service.Services
{
    public class UserService : IUserService
    {
        private const int MaxPageSize = 50;

        private readonly IUserRepository _users;
        private readonly IMealInvitationRepository _invitations;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository users,
            IMealInvitationRepository invitations,
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _users = users;
            _invitations = invitations;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
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
    }
}