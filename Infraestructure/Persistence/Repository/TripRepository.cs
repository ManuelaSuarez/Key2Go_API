using Application.Abstraction;
using Domain.Entity;

namespace Infraestructure.Persistence.Repository
{
    public class TripRepository : BaseRepository<Trip>, ITripRepository
    {
        public TripRepository(Key2GoDbContext context) : base(context)
        {
        }
    }
}
