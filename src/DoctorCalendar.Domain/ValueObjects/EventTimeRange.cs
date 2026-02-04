using DoctorCalendar.Domain.Exceptions;

namespace DoctorCalendar.Domain.ValueObjects;

public sealed record EventTimeRange
{
	public DateTime StartUtc { get; }
	public DateTime EndUtc { get; }

	private EventTimeRange(DateTime startUtc, DateTime endUtc)
	{
		StartUtc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
		EndUtc = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);
	}

	public static EventTimeRange Create(DateTime startUtc, DateTime endUtc)
	{
		if (startUtc == default || endUtc == default)
			throw new DomainValidationException("Start and end time are required.");

		var start = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
		var end = DateTime.SpecifyKind(endUtc, DateTimeKind.Utc);

		if (end <= start)
			throw new DomainValidationException("End time must be after start time.");

		return new EventTimeRange(start, end);
	}

	public bool Overlaps(EventTimeRange other) =>
		StartUtc < other.EndUtc && EndUtc > other.StartUtc;
}
