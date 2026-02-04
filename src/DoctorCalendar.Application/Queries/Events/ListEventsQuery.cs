using DoctorCalendar.Application.Dtos;
using MediatR;

namespace DoctorCalendar.Application.Queries.Events;

public sealed record ListEventsQuery(
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Status,
    string? Q
) : IRequest<IReadOnlyList<EventDto>>;
