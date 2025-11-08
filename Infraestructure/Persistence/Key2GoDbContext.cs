using Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence
{
    public class Key2GoDbContext : DbContext
    {
        public Key2GoDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Trip> Trips { get; set; }
    }
}
