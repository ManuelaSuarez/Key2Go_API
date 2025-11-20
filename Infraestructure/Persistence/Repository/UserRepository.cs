using Application.Abstraction;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly Key2GoDbContext _context;
        public UserRepository(Key2GoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticatorAsync(string email, string password)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == password && u.IsActive);
        }
        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetByDni(string dni)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Dni == dni);
        }
    }
}
