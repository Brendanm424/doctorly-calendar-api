using System.Text.RegularExpressions;
using DoctorCalendar.Domain.Exceptions;

namespace DoctorCalendar.Domain.ValueObjects;

public sealed record EmailAddress
{
    private static readonly Regex BasicEmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainValidationException("Email address is required.");

        var trimmed = value.Trim();

        if (trimmed.Length > 254)
            throw new DomainValidationException("Email address must be 254 characters or fewer.");

        if (!BasicEmailRegex.IsMatch(trimmed))
            throw new DomainValidationException("Email address format is invalid.");

        return new EmailAddress(trimmed);
    }

    public override string ToString() => Value;
}
