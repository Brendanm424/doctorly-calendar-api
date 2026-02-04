using DoctorCalendar.Application.Dtos;
using MediatR;

namespace DoctorCalendar.Application.Queries.Events;

public sealed record GetEventByIdQuery(Guid Id) : IRequest<EventDto?>;
