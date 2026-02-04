using DoctorCalendar.Application.Commands.CreateEvent;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DoctorCalendar.Api.Controllers;

[ApiController]
[Route("events")]
public sealed class EventsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEventCommand command, CancellationToken ct)
    {
        var id = await _mediator.Send(command, ct);

        // We'll add GET /events/{id} in Task 3.4, but Location is still good practice
        return Created($"/events/{id}", new { id });
    }
}
