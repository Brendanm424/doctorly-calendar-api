namespace DoctorCalendar.Domain.Events;

public sealed record EventCreatedDomainEvent(Guid EventId) : IDomainEvent
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
