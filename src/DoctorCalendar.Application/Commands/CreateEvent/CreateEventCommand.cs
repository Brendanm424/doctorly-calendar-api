using MediatR;

namespace DoctorCalendar.Application.Commands.CreateEvent;

public sealed record CreateEventCommand(
    string Title,
    string? Description,
    DateTime StartUtc,
    DateTime EndUtc,
    IReadOnlyList<CreateEventAttendee> Attendees
) : IRequest<Guid>;

public sealed record CreateEventAttendee(
    string Name,
    string Email
);
