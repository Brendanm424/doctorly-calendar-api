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
}
