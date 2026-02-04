using DoctorCalendar.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DoctorCalendar.Infrastructure.Persistence.Configurations;

public sealed class EventAttendeeConfiguration : IEntityTypeConfiguration<EventAttendee>
{
    public void Configure(EntityTypeBuilder<EventAttendee> builder)
    {
        builder.ToTable("EventAttendees");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        // Value Object: EmailAddress
        builder.OwnsOne(x => x.Email, email =>
        {
            email.Property(p => p.Value)
                .HasColumnName("Email")
                .HasMaxLength(254)
                .IsRequired();
        });

        // Shadow FK created by HasForeignKey("EventId") in CalendarEvent config.
        // Define a typed shadow property so it can be indexed reliably.
        builder.Property<Guid>("EventId");

        // Define a typed shadow property for the owned column to index it
        builder.Property<string>("EmailAddress")
            .HasMaxLength(254);

        builder.HasIndex("EventId", "EmailAddress");
    }
}
