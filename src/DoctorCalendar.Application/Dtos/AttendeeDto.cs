namespace DoctorCalendar.Application.Dtos;

public sealed record AttendeeDto(
    Guid Id,
    string Name,
    string Email,
    string Status
);
