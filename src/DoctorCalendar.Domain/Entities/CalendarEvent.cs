using DoctorCalendar.Domain.Events;
using DoctorCalendar.Domain.Exceptions;
using DoctorCalendar.Domain.ValueObjects;

namespace DoctorCalendar.Domain.Entities;

public class CalendarEvent
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 2000;

    private readonly List<EventAttendee> _attendees = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public EventTimeRange TimeRange { get; private set; } = default!;
    public EventStatus Status { get; private set; } = EventStatus.Active;

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    public IReadOnlyCollection<EventAttendee> Attendees => _attendees.AsReadOnly();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    // For EF Core
    private CalendarEvent() { }

    private CalendarEvent(string title, string? description, EventTimeRange timeRange, IEnumerable<EventAttendee> attendees)
    {
        Title = title;
        Description = description;
        TimeRange = timeRange;

        _attendees.AddRange(attendees);
    }

    public static CalendarEvent Create(
        string title,
        string? description,
        DateTime startUtc,
        DateTime endUtc,
        IEnumerable<(string Name, string Email)> attendees)
    {
        var normalizedTitle = NormalizeTitle(title);
        var normalizedDescription = NormalizeDescription(description);

        var timeRange = EventTimeRange.Create(startUtc, endUtc);
        EnsureNotTooFarInFuture(timeRange.StartUtc);

        var attendeeEntities = attendees?.Select(a => EventAttendee.Create(a.Name, a.Email)).ToList()
                              ?? new List<EventAttendee>();

        EnsureNoDuplicateEmails(attendeeEntities);

        var evt = new CalendarEvent(normalizedTitle, normalizedDescription, timeRange, attendeeEntities);
        evt.AddDomainEvent(new EventCreatedDomainEvent(evt.Id));
        return evt;
    }

    public void UpdateDetails(
        string title,
        string? description,
        DateTime startUtc,
        DateTime endUtc,
        IEnumerable<(Guid? AttendeeId, string Name, string Email, AttendanceStatus? Status)> attendees)
    {
        EnsureActive();

        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);

        var newRange = EventTimeRange.Create(startUtc, endUtc);
        EnsureNotTooFarInFuture(newRange.StartUtc);
        TimeRange = newRange;

        // Simple, time-boxed approach: replace attendee list.
        // Preserves supplied Status when provided; otherwise defaults to Invited.
        var rebuilt = new List<EventAttendee>();
        foreach (var a in attendees ?? Enumerable.Empty<(Guid?, string, string, AttendanceStatus?)>())
        {
            var attendee = EventAttendee.Create(a.Name, a.Email);

            if (a.Status is AttendanceStatus.Accepted or AttendanceStatus.Rejected)
                attendee.Respond(a.Status.Value);

            rebuilt.Add(attendee);
        }

        EnsureNoDuplicateEmails(rebuilt);

        _attendees.Clear();
        _attendees.AddRange(rebuilt);

        UpdatedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new EventUpdatedDomainEvent(Id));
    }

    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            return;

        Status = EventStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new EventCancelledDomainEvent(Id));
    }

    public void Respond(Guid attendeeId, AttendanceStatus response)
    {
        EnsureActive();

        var attendee = _attendees.SingleOrDefault(a => a.Id == attendeeId);
        if (attendee is null)
            throw new DomainValidationException("Attendee not found.");

        attendee.Respond(response);
        UpdatedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new EventUpdatedDomainEvent(Id));
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    private void EnsureActive()
    {
        if (Status != EventStatus.Active)
            throw new DomainValidationException("Event is cancelled and cannot be modified.");
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainValidationException("Title is required.");

        var trimmed = title.Trim();

        if (trimmed.Length > TitleMaxLength)
            throw new DomainValidationException($"Title must be {TitleMaxLength} characters or fewer.");

        return trimmed;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (description is null)
            return null;

        var trimmed = description.Trim();

        if (trimmed.Length > DescriptionMaxLength)
            throw new DomainValidationException($"Description must be {DescriptionMaxLength} characters or fewer.");

        return trimmed.Length == 0 ? null : trimmed;
    }

    private static void EnsureNoDuplicateEmails(IEnumerable<EventAttendee> attendees)
    {
        var duplicates = attendees
            .GroupBy(a => a.Email.Value, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count > 0)
            throw new DomainValidationException("Duplicate attendee email addresses are not allowed.");
    }

    private static void EnsureNotTooFarInFuture(DateTime startUtc)
    {
        var max = DateTime.UtcNow.AddDays(180);
        if (startUtc > max)
            throw new DomainValidationException("Events can only be scheduled up to 180 days in advance.");
    }

    private void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);
}
