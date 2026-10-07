using Microsoft.EntityFrameworkCore;
using CollegeBusManagement.Models;

namespace CollegeBusManagement.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<BusRoute> BusRoutes => Set<BusRoute>();
    public DbSet<BusStop> BusStops => Set<BusStop>();
    public DbSet<BusPass> BusPasses => Set<BusPass>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // BusRoute Configuration
        modelBuilder.Entity<BusRoute>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.RouteName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(r => r.StartPoint)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(r => r.EndPoint)
                  .IsRequired()
                  .HasMaxLength(100);

            // Configure decimal precision
            entity.Property(r => r.MonthlyFee)
                  .HasPrecision(18, 2);

            entity.Property(r => r.TotalSeats)
                  .IsRequired();

            entity.Property(r => r.AvailableSeats)
                  .IsRequired();
        });

        // BusStop Configuration & Relationship
        modelBuilder.Entity<BusStop>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.StopName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(s => s.PickupTime)
                  .IsRequired()
                  .HasMaxLength(50);

            // One-to-Many relationship with BusRoute
            // Cascade Delete: Deleting a route deletes all its intermediate stops
            entity.HasOne(s => s.BusRoute)
                  .WithMany(r => r.BusStops)
                  .HasForeignKey(s => s.BusRouteId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // BusPass Configuration & Relationship
        modelBuilder.Entity<BusPass>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.StudentName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(p => p.RollNumber)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(p => p.Email)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(p => p.ContactNumber)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(p => p.Status)
                  .IsRequired()
                  .HasMaxLength(20);

            // One-to-Many relationship with BusRoute
            // Restrict Delete: Prevents deleting a route if student bus passes are associated with it
            entity.HasOne(p => p.BusRoute)
                  .WithMany(r => r.BusPasses)
                  .HasForeignKey(p => p.BusRouteId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
