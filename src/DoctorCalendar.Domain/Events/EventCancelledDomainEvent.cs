namespace DoctorCalendar.Domain.Events;

public sealed record EventCancelledDomainEvent(Guid EventId) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
