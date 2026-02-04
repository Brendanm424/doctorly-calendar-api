using DoctorCalendar.Application.Commands.CancelEvent;
using DoctorCalendar.Application.Commands.CreateEvent;
using DoctorCalendar.Application.Commands.UpdateEvent;
using DoctorCalendar.Application.Queries.Events;
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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
    [FromRoute] Guid id,
    [FromQuery] int version,
    CancellationToken ct)
    {
        await _mediator.Send(new CancelEventCommand(id, version), ct);
        return NoContent();
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        [FromQuery] string? status,
        [FromQuery] string? q,
        CancellationToken ct)
    {
        var results = await _mediator.Send(new ListEventsQuery(fromUtc, toUtc, status, q), ct);
        return Ok(results);
    }

}
