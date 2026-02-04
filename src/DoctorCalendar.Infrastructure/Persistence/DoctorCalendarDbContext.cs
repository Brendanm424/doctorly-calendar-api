using DoctorCalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Infrastructure.Persistence;

public sealed class DoctorCalendarDbContext : DbContext
{
    public DoctorCalendarDbContext(DbContextOptions<DoctorCalendarDbContext> options)
        : base(options) { }

    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DoctorCalendarDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public DoctorCalendarDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DoctorCalendarDbContext>();

        // Keep consistent with appsettings.json
        optionsBuilder.UseSqlite("Data Source=doctorcalendar.db");

        return new DoctorCalendarDbContext(optionsBuilder.Options);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<CalendarEvent>())
        {
            if (entry.State == EntityState.Modified)
            {
                var current = entry.Property<int>("Version").CurrentValue;
                entry.Property<int>("Version").CurrentValue = current + 1;
            }
        }

        return base.SaveChangesAsync(ct);
    }
}
