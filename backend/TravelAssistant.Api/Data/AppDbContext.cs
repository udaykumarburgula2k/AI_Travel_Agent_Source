using Microsoft.EntityFrameworkCore;
using TravelAssistant.Api.Models;

namespace TravelAssistant.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<TripPlan> TripPlans => Set<TripPlan>();
    public DbSet<DayPlan> DayPlans => Set<DayPlan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TripPlan>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Destination).HasMaxLength(150).IsRequired();
            entity.Property(x => x.BudgetLevel).HasMaxLength(50).IsRequired();
            entity.Property(x => x.TravelerType).HasMaxLength(50);
            entity.Property(x => x.BudgetBand).HasMaxLength(100);
            entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
            entity.HasMany(x => x.DayPlans).WithOne(x => x.TripPlan).HasForeignKey(x => x.TripPlanId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<DayPlan>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.TripPlanId, x.DayNumber }).IsUnique();
        });
    }
}
