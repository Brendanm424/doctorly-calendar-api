using DoctorCalendar.Domain.Entities;
using MediatR;

namespace DoctorCalendar.Application.Commands.UpdateEvent;

public sealed record UpdateEventCommand(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartUtc,
    DateTime EndUtc,
    IReadOnlyList<UpdateEventAttendee> Attendees,
    int Version
) : IRequest;

public sealed record UpdateEventAttendee(
    Guid? Id,
    string Name,
    string Email,
    AttendanceStatus? Status
);
