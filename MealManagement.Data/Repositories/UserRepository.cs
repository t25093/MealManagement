using MealManagement.Core.DTOs;
using MealManagement.Core.Entities;
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
    }
}