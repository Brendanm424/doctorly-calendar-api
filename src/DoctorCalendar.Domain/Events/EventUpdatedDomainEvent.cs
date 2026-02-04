namespace DoctorCalendar.Domain.Events;

public sealed record EventUpdatedDomainEvent(Guid EventId) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
