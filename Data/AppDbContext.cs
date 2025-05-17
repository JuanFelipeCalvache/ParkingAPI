using Microsoft.EntityFrameworkCore;
using Parking.Models;

namespace Parking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<EntryExit> EntryExits { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.NumberPlate)
                .IsUnique();

        }


    }
}
