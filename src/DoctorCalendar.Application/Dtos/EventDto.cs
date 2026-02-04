namespace DoctorCalendar.Application.Dtos;

public sealed record EventDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime StartUtc,
    DateTime EndUtc,
    string Status,
    IReadOnlyList<AttendeeDto> Attendees,
    string? RowVersion // base64 (shadow property via EF, populated in queries)
);
