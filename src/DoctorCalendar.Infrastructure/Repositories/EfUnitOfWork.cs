using DoctorCalendar.Application.Interfaces;
using DoctorCalendar.Infrastructure.Persistence;

namespace DoctorCalendar.Infrastructure.Repositories;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly DoctorCalendarDbContext _db;

    public EfUnitOfWork(DoctorCalendarDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return _db.SaveChangesAsync(ct);
    }
}
