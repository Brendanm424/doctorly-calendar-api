using DoctorCalendar.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace DoctorCalendar.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new { error = ex.Message });
            await context.Response.WriteAsync(payload);
        }
    }
}
