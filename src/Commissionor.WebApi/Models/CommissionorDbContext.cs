using Microsoft.EntityFrameworkCore;

namespace Commissionor.WebApi.Models
{
    public class CommissionorDbContext : DbContext
    {
        public DbSet<Reader> Readers { get; set; }
        public DbSet<Location> Locations { get; set; }

        public CommissionorDbContext(DbContextOptions<CommissionorDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reader>()
                        .HasMany(r => r.Locations)
                        .WithOne()
                        .HasForeignKey(l => l.ReaderId)
                        .HasConstraintName("FK_Location_Reader")
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
