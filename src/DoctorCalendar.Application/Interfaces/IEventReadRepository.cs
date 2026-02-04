using DoctorCalendar.Application.Dtos;

namespace DoctorCalendar.Application.Interfaces;

public interface IEventReadRepository
{
    Task<EventDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<EventDto>> ListAsync(
        DateTime? fromUtc,
        DateTime? toUtc,
        string? status,
        string? q,
        CancellationToken ct);
}
