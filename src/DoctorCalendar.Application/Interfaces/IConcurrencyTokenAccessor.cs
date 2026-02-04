namespace DoctorCalendar.Application.Interfaces;

public interface IConcurrencyTokenAccessor
{
    void SetOriginalRowVersion<TEntity>(TEntity entity, int version)
        where TEntity : class;
}
