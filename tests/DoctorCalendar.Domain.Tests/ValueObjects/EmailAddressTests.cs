using DoctorCalendar.Domain.Exceptions;
using DoctorCalendar.Domain.ValueObjects;
using FluentAssertions;

namespace DoctorCalendar.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEmpty()
    {
        var act = () => EmailAddress.Create(" ");

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*required*");
    }

    [Fact]
    public void Create_ShouldThrow_WhenInvalidFormat()
    {
        var act = () => EmailAddress.Create("not-an-email");

        act.Should().Throw<DomainValidationException>()
            .WithMessage("*format*");
    }

    [Fact]
    public void Create_ShouldNormalize_ByTrimming()
    {
        var email = EmailAddress.Create("  alice@example.com  ");

        email.Value.Should().Be("alice@example.com");
    }
}
