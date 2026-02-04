using DoctorCalendar.Domain.Exceptions;
using DoctorCalendar.Domain.ValueObjects;

namespace DoctorCalendar.Domain.Entities;

public class EventAttendee
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = default!;
    public EmailAddress Email { get; private set; } = default!;
    public AttendanceStatus Status { get; private set; } = AttendanceStatus.Invited;

    // For EF Core
    private EventAttendee() { }

    private EventAttendee(string name, EmailAddress email)
    {
        Name = name;
        Email = email;
    }

    public static EventAttendee Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException("Attendee name is required.");

        var trimmed = name.Trim();

        if (trimmed.Length > 120)
            throw new DomainValidationException("Attendee name must be 120 characters or fewer.");

        return new EventAttendee(trimmed, EmailAddress.Create(email));
    }

    public void Respond(AttendanceStatus status)
    {
        if (status is AttendanceStatus.Invited)
            throw new DomainValidationException("Response must be Accepted or Rejected.");

        Status = status;
    }
}
