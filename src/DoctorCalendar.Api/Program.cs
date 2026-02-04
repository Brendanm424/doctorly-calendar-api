using DoctorCalendar.Application.Commands.CreateEvent;
using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Infrastructure.Persistence;
using DoctorCalendar.Infrastructure.Repositories;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using DoctorCalendar.Api.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DoctorCalendarDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DoctorCalendar"));
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine);

});

var cs = builder.Configuration.GetConnectionString("DoctorCalendar")!;
var dbPath = cs.Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase).Trim();
Console.WriteLine($"[DoctorCalendar] SQLite DB path = {Path.GetFullPath(dbPath)}");


builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
builder.Services.AddScoped<IConcurrencyTokenAccessor, EfConcurrencyTokenAccessor>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateEventCommand).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(CreateEventCommandValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddDbContext<DoctorCalendarDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DoctorCalendar")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();
