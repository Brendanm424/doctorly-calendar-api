using DoctorCalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoctorCalendar.Infrastructure.Persistence.Configurations;

public sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(CalendarEvent.TitleMaxLength)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(CalendarEvent.DescriptionMaxLength);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc);
        builder.Property(x => x.CancelledAtUtc);

        // Value Object: EventTimeRange
        builder.OwnsOne(x => x.TimeRange, tr =>
        {
            tr.Property(p => p.StartUtc)
                .HasColumnName("StartUtc")
                .IsRequired();

            tr.Property(p => p.EndUtc)
                .HasColumnName("EndUtc")
                .IsRequired();
        });

        // Attendees collection
        builder.HasMany(x => x.Attendees)
            .WithOne()
            .HasForeignKey("EventId")
            .OnDelete(DeleteBehavior.Cascade);

        // SQLlite...not supporting RowVersion.
        builder.Property<int>("Version")
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        // DomainEvents are not persisted
        builder.Ignore(x => x.DomainEvents);
    }
}
