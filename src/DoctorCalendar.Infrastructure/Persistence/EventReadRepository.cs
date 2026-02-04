using DoctorCalendar.Application.Dtos;
using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Infrastructure.Persistence;

public sealed class EventReadRepository : IEventReadRepository
{
    private readonly DoctorCalendarDbContext _db;

    public EventReadRepository(DoctorCalendarDbContext db)
    {
        _db = db;
    }

    public async Task<EventDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Set<CalendarEvent>()
            .AsNoTracking()
            .Include(x => x.Attendees)
            .Where(x => x.Id == id)
            .Select(x => new EventDto(
                x.Id,
                x.Title,
                x.Description,
                x.TimeRange.StartUtc,
                x.TimeRange.EndUtc,
                x.Status.ToString(),
                x.Attendees.Select(a => new AttendeeDto(
                    a.Id,
                    a.Name,
                    a.Email.Value,
                    a.Status.ToString()
                )).ToList(),
                EF.Property<int>(x, "Version")
            ))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<EventDto>> ListAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        string? status,
        string? q,
        CancellationToken ct)
    {
        var query = _db.Set<CalendarEvent>()
            .AsNoTracking()
            .Include(x => x.Attendees)
            .AsQueryable();

        if (fromUtc is not null)
            query = query.Where(x => x.TimeRange.EndUtc >= fromUtc.Value);

        if (toUtc is not null)
            query = query.Where(x => x.TimeRange.StartUtc <= toUtc.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status.ToString() == status);

        if (!string.IsNullOrWhiteSpace(q))
        {
            query = query.Where(x =>
                x.Title.Contains(q) ||
                (x.Description != null && x.Description.Contains(q)) ||
                x.Attendees.Any(a =>
                    a.Name.Contains(q) ||
                    a.Email.Value.Contains(q)));
        }

        return await query
            .OrderBy(x => x.TimeRange.StartUtc)
            .Select(x => new EventDto(
                x.Id,
                x.Title,
                x.Description,
                x.TimeRange.StartUtc,
                x.TimeRange.EndUtc,
                x.Status.ToString(),
                x.Attendees.Select(a => new AttendeeDto(
                    a.Id,
                    a.Name,
                    a.Email.Value,
                    a.Status.ToString()
                )).ToList(),
                EF.Property<int>(x, "Version")
            ))
            .ToListAsync(ct);
    }
}
