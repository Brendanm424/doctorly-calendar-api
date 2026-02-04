using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Infrastructure.Persistence;
using DoctorCalendar.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DoctorCalendarDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DoctorCalendar"));
});

builder.Services.AddScoped<ICalendarEventRepository, CalendarEventRepository>();
builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
