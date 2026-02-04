using DoctorCalendar.Application.Dtos;
using DoctorCalendar.Application.Interfaces;
using MediatR;

namespace DoctorCalendar.Application.Queries.Events;

public sealed class ListEventsQueryHandler
    : IRequestHandler<ListEventsQuery, IReadOnlyList<EventDto>>
{
    private readonly IEventReadRepository _repo;

    public ListEventsQueryHandler(IEventReadRepository repo)
    {
        _repo = repo;
    }

    public Task<IReadOnlyList<EventDto>> Handle(
        ListEventsQuery request,
        CancellationToken ct)
        => _repo.ListAsync(
            request.FromUtc,
            request.ToUtc,
            request.Status,
            request.Q,
            ct);
}
