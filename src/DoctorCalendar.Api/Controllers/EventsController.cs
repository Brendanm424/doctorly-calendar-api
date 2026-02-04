using DoctorCalendar.Application.Commands.CreateEvent;
using DoctorCalendar.Application.Commands.UpdateEvent;
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

        return Created($"/events/{id}", new { id });
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
    Guid id,
    [FromBody] UpdateEventCommand command,
    CancellationToken ct)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match payload id.");

        await _mediator.Send(command, ct);
        return NoContent();
    }

}
