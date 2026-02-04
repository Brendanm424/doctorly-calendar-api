using DoctorCalendar.Domain.Exceptions;
using DoctorCalendar.Domain.ValueObjects;
using FluentAssertions;

namespace DoctorCalendar.Domain.Tests.ValueObjects;

public class EventTimeRangeTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEndNotAfterStart()
    {
        var start = DateTime.UtcNow;
        var end = start;

        var act = () => EventTimeRange.Create(start, end);

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*End time must be after start time*");
    }

    [Fact]
    public void Overlaps_ShouldReturnTrue_WhenRangesOverlap()
    {
        var a = EventTimeRange.Create(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var b = EventTimeRange.Create(DateTime.UtcNow.AddHours(1.5), DateTime.UtcNow.AddHours(2.5));

        a.Overlaps(b).Should().BeTrue();
    }

    [Fact]
    public void Overlaps_ShouldReturnFalse_WhenRangesDoNotOverlap()
    {
        var a = EventTimeRange.Create(DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var b = EventTimeRange.Create(DateTime.UtcNow.AddHours(2), DateTime.UtcNow.AddHours(3)); // touching edge

        a.Overlaps(b).Should().BeFalse();
    }
}
