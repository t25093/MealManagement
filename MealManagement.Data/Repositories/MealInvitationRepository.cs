using MealManagement.Core.Entities;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MealManagement.Data.Repositories
{
    public class MealInvitationRepository : IMealInvitationRepository
    {
        private readonly AppDbContext _context;

        public MealInvitationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MealInvitation>> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.MealInvitations
                .Include(i => i.Meal)
                .Where(i => i.Email == email)
                .ToListAsync(ct);
        }

        public void RemoveRange(IEnumerable<MealInvitation> invitations)
        {
            _context.MealInvitations.RemoveRange(invitations);
        }
    }
}
