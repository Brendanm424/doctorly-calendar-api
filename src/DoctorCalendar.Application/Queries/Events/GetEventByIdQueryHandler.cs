using DoctorCalendar.Application.Dtos;
using DoctorCalendar.Application.Interfaces;
using MediatR;

namespace DoctorCalendar.Application.Queries.Events;

public sealed class GetEventByIdQueryHandler
    : IRequestHandler<GetEventByIdQuery, EventDto?>
{
    private readonly IEventReadRepository _repo;

    public GetEventByIdQueryHandler(IEventReadRepository repo)
    {
        _repo = repo;
    }

    public Task<EventDto?> Handle(GetEventByIdQuery request, CancellationToken ct)
        => _repo.GetByIdAsync(request.Id, ct);
}
