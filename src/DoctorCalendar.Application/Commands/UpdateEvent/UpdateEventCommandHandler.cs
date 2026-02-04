using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Application.Commands.UpdateEvent;

public sealed class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
    private readonly ICalendarEventRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IConcurrencyTokenAccessor _concurrency;

    public UpdateEventCommandHandler(
    ICalendarEventRepository repo,
    IUnitOfWork uow,
    IConcurrencyTokenAccessor concurrency)
    {
        _repo = repo;
        _uow = uow;
        _concurrency = concurrency;
    }


    public async Task Handle(UpdateEventCommand request, CancellationToken ct)
    {
        var evt = await _repo.GetForUpdateAsync(request.Id, ct);
        if (evt is null)
            throw new DomainValidationException("Event not found.");

        // Apply original RowVersion for optimistic concurrency
        _concurrency.SetOriginalRowVersion(evt, request.Version);

        evt.UpdateDetails(
            request.Title,
            request.Description,
            request.StartUtc,
            request.EndUtc,
            request.Attendees.Select(a =>
                (a.Id, a.Name, a.Email, a.Status))
        );

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
