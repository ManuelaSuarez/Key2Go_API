using Application.Abstraction;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repository
{
    public class TripRepository : BaseRepository<Trip>, ITripRepository
    {
        private readonly Key2GoDbContext _context;
        public TripRepository(Key2GoDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Trip>> GetByStatusAsync(int status)
        {
            return await _context.Trips
                .Where(u => (int)u.Status == status)
                .ToListAsync();
        }
    }
}
