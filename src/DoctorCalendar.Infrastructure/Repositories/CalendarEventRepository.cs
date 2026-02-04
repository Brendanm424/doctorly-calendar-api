using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Domain.Entities;
using DoctorCalendar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Infrastructure.Repositories;

public sealed class CalendarEventRepository : ICalendarEventRepository
{
    private readonly DoctorCalendarDbContext _db;

    public CalendarEventRepository(DoctorCalendarDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
    {
        await _db.CalendarEvents.AddAsync(calendarEvent, ct);
    }

    public async Task<CalendarEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.CalendarEvents
            .AsNoTracking()
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<CalendarEvent?> GetForUpdateAsync(Guid id, CancellationToken ct = default)
    {
        return await _db.CalendarEvents
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public IQueryable<CalendarEvent> Query()
    {
        return _db.CalendarEvents
            .AsNoTracking()
            .Include(e => e.Attendees);
    }
}
