using MediatR;

namespace DoctorCalendar.Application.Commands.CancelEvent;

public sealed record CancelEventCommand(Guid Id, int Version) : IRequest;
