using DoctorCalendar.Domain.Entities;
using DoctorCalendar.Domain.Exceptions;
using FluentAssertions;

namespace DoctorCalendar.Domain.Tests.Entities;

public class CalendarEventTests
{
    [Fact]
    public void Create_ShouldThrow_WhenTitleIsEmpty()
    {
        var act = () => CalendarEvent.Create(
            title: " ",
            description: "desc",
            startUtc: DateTime.UtcNow.AddHours(1),
            endUtc: DateTime.UtcNow.AddHours(2),
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Title*required*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEndIsNotAfterStart()
    {
        var start = DateTime.UtcNow.AddHours(2);
        var end = start;

        var act = () => CalendarEvent.Create(
            title: "Consultation",
            description: "desc",
            startUtc: start,
            endUtc: end,
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*End time must be after start time*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenScheduledMoreThan180DaysAhead()
    {
        var start = DateTime.UtcNow.AddDays(181);
        var end = start.AddHours(1);

        var act = () => CalendarEvent.Create(
            title: "Future event",
            description: null,
            startUtc: start,
            endUtc: end,
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*180 days*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenDuplicateAttendeeEmails()
    {
        var act = () => CalendarEvent.Create(
            title: "Team meeting",
            description: "desc",
            startUtc: DateTime.UtcNow.AddHours(1),
            endUtc: DateTime.UtcNow.AddHours(2),
            attendees: new[]
            {
                ("Alice", "alice@example.com"),
                ("Bob", "ALICE@example.com")
            }
        );

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*Duplicate attendee email*");
    }

    [Fact]
    public void Cancel_ShouldSetStatusCancelled_AndBeIdempotent()
    {
        var evt = CalendarEvent.Create(
            title: "Appointment",
            description: null,
            startUtc: DateTime.UtcNow.AddHours(1),
            endUtc: DateTime.UtcNow.AddHours(2),
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        evt.Cancel();
        evt.Status.Should().Be(EventStatus.Cancelled);
        evt.CancelledAtUtc.Should().NotBeNull();

        // idempotent
        var cancelledAt = evt.CancelledAtUtc;
        evt.Cancel();
        evt.CancelledAtUtc.Should().Be(cancelledAt);
    }

    [Fact]
    public void Respond_ShouldThrow_WhenEventIsCancelled()
    {
        var evt = CalendarEvent.Create(
            title: "Appointment",
            description: null,
            startUtc: DateTime.UtcNow.AddHours(1),
            endUtc: DateTime.UtcNow.AddHours(2),
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        var attendeeId = evt.Attendees.Single().Id;

        evt.Cancel();

        var act = () => evt.Respond(attendeeId, AttendanceStatus.Accepted);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*cancelled*");
    }

    [Fact]
    public void Respond_ShouldSetAttendeeStatus_WhenAccepted()
    {
        var evt = CalendarEvent.Create(
            title: "Appointment",
            description: null,
            startUtc: DateTime.UtcNow.AddHours(1),
            endUtc: DateTime.UtcNow.AddHours(2),
            attendees: new[] { ("Alice", "alice@example.com") }
        );

        var attendee = evt.Attendees.Single();
        attendee.Status.Should().Be(AttendanceStatus.Invited);

        evt.Respond(attendee.Id, AttendanceStatus.Accepted);

        evt.Attendees.Single().Status.Should().Be(AttendanceStatus.Accepted);
        evt.UpdatedAtUtc.Should().NotBeNull();
    }
}
