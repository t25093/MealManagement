using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
using MealManagement.Core.Enums;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email, ct);
        }

        public async Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Users.AsNoTracking();

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderBy(u => u.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<User>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task AddAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        public async Task<User?> GetForDeleteAsync(int id, CancellationToken ct = default)
        {
            return await _context.Users
                .Include(u => u.Assignments)
                    .ThenInclude(a => a.MealDish)
                .Include(u => u.Messages)
                .FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<int?> GetAnyAdminIdAsync(CancellationToken ct = default)
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Admin)
                .OrderBy(u => u.Id)
                .Select(u => (int?)u.Id)
                .FirstOrDefaultAsync(ct);
        }

        public async Task RemoveWithRelatedDataAsync(User user, int dishesNewOwnerId, CancellationToken ct = default)
        {
            // מאכלים שהוסיפה נשארים במאגר המשותף, כי ייתכן שהם בשימוש בארוחות של אחרות
            var dishes = await _context.Dishes
                .Where(d => d.CreatedByUserId == user.Id)
                .ToListAsync(ct);
            foreach (var dish in dishes)
                dish.CreatedByUserId = dishesNewOwnerId;

            // לקיחות, הודעות ומתכונים: מוגדרים Restrict, ולכן נמחקים במפורש לפני המשתמשת
            _context.UserDishAssignments.RemoveRange(user.Assignments);
            _context.ChatMessages.RemoveRange(user.Messages);

            var recipes = await _context.Recipes
                .Where(r => r.AuthorId == user.Id)
                .ToListAsync(ct);
            _context.Recipes.RemoveRange(recipes);

            // הארוחות שבבעלותה נמחקות, ואיתן המנות, הלקיחות, ההודעות, המתכונים וההזמנות (Cascade)
            var ownedMeals = await _context.Meals
                .Where(m => m.OwnerId == user.Id)
                .ToListAsync(ct);
            _context.Meals.RemoveRange(ownedMeals);

            // ההשתתפות בארוחות של אחרות (MealParticipants) נמחקת ב-Cascade
            _context.Users.Remove(user);
        }
    }
}