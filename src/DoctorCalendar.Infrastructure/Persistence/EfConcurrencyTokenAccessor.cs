using DoctorCalendar.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Infrastructure.Persistence;

public sealed class EfConcurrencyTokenAccessor : IConcurrencyTokenAccessor
{
    private readonly DoctorCalendarDbContext _db;

    public EfConcurrencyTokenAccessor(DoctorCalendarDbContext db)
    {
        _db = db;
    }

    public void SetOriginalRowVersion<TEntity>(TEntity entity, int version)
        where TEntity : class
    {
        _db.Entry(entity).Property<int>("Version").OriginalValue = version;
    }
}
