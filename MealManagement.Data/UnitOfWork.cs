using MealManagement.Core.Exceptions;
using MealManagement.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace MealManagement.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new ConflictException(
                    "הנתונים עודכנו במקביל על ידי משתמשת אחרת. יש לרענן ולנסות שוב.");
            }
            catch (DbUpdateException ex)
                when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                throw new ConflictException("הרשומה כבר קיימת.");
            }
        }
    }
}