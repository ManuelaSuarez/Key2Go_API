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

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Dni = "11111111", Name = "User", Surname = "User", Email = "user@mail.com", Password = "user", PhoneNumber = "3411111111", RoleId = (int)RoleType.User },
                new User { Id = 2, Dni = "22222222", Name = "Admin", Surname = "Admin", Email = "admin@mail.com", Password = "admin", PhoneNumber = "342222222", RoleId = (int)RoleType.Admin },
                new User { Id = 3, Dni = "33333333", Name = "Super", Surname = "Admin", Email = "superadmin@mail.com", Password = "superadmin", PhoneNumber = "343333333", RoleId = (int)RoleType.SuperAdmin }
            );

            modelBuilder.Entity<Car>().HasData(
                new Car { Id = 1, LicensePlate = "AB 123 CD", Brand = "Toyota", Model = "Corolla", YearOfManufacture = 2018, Km  = 90000, DailyPriceUsd = 50, Status = CarStatus.Available },
                new Car { Id = 2, LicensePlate = "AC 124 DC", Brand = "Ford", Model = "Focus", YearOfManufacture = 2020, Km = 150000, DailyPriceUsd = 60, Status = CarStatus.Available },
                new Car { Id = 3, LicensePlate = "AB 125 CD", Brand = "Audi", Model = "A4", YearOfManufacture = 2019, Km = 30000, DailyPriceUsd = 100, Status = CarStatus.Available }
            );


            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Payment)
                .WithOne(p => p.Trip)
                .HasForeignKey<Payment>(p => p.TripId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}