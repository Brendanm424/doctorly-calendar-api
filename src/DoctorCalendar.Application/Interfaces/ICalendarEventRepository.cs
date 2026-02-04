using DoctorCalendar.Domain.Entities;

namespace DoctorCalendar.Application.Interfaces;

public interface ICalendarEventRepository
{
    Task AddAsync(CalendarEvent calendarEvent, CancellationToken ct = default);

    Task<CalendarEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<CalendarEvent?> GetForUpdateAsync(Guid id, CancellationToken ct = default);

    IQueryable<CalendarEvent> Query();
}
