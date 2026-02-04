using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Domain.Entities;
using MediatR;

namespace DoctorCalendar.Application.Commands.CreateEvent;

public sealed class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
{
    private readonly ICalendarEventRepository _repo;
    private readonly IUnitOfWork _uow;

    public CreateEventCommandHandler(ICalendarEventRepository repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken ct)
    {
        var evt = CalendarEvent.Create(
            title: request.Title,
            description: request.Description,
            startUtc: request.StartUtc,
            endUtc: request.EndUtc,
            attendees: request.Attendees.Select(a => (a.Name, a.Email))
        );

        await _repo.AddAsync(evt, ct);
        await _uow.SaveChangesAsync(ct);

        return evt.Id;
    }
}
