using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Application.Commands.CancelEvent;

public sealed class CancelEventCommandHandler : IRequestHandler<CancelEventCommand>
{
    private readonly ICalendarEventRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IConcurrencyTokenAccessor _concurrency;

    public CancelEventCommandHandler(
        ICalendarEventRepository repo,
        IUnitOfWork uow,
        IConcurrencyTokenAccessor concurrency)
    {
        _repo = repo;
        _uow = uow;
        _concurrency = concurrency;
    }

    public async Task Handle(CancelEventCommand request, CancellationToken ct)
    {
        var evt = await _repo.GetForUpdateAsync(request.Id, ct);
        if (evt is null)
            throw new DomainValidationException("Event not found.");

        // optimistic concurrency
        _concurrency.SetOriginalRowVersion(evt, request.Version);

        evt.Cancel();

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DomainValidationException(
                "The event was updated by another operation. Please reload and retry.");
        }
    }
}
