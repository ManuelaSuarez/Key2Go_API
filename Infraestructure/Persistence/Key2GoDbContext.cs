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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = RoleType.User },
                new Role { Id = 2, Name = RoleType.Admin },
                new Role { Id = 3, Name = RoleType.SuperAdmin }
            );
        }
    }
}