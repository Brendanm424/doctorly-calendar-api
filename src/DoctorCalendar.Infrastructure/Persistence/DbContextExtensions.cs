using Microsoft.EntityFrameworkCore;

namespace DoctorCalendar.Infrastructure.Persistence;

public static class DbContextExtensions
{
    public static int GetRowVersion<TEntity>(this DbContext db, TEntity entity)
        where TEntity : class
    {
        return db.Entry(entity).Property<int>("Version").CurrentValue;
    }

    public static void SetOriginalRowVersion<TEntity>(this DbContext db, TEntity entity, int version)
        where TEntity : class
    {
        db.Entry(entity).Property<int>("Version").OriginalValue = version;
    }
}
