using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;

namespace Sevak.Infrastructure.Data.Configurations;

public class EventVolunteerConfiguration : IEntityTypeConfiguration<EventVolunteer>
{
    public void Configure(EntityTypeBuilder<EventVolunteer> builder)
    {
        builder.Property(ev => ev.Status)
            .HasConversion<string>()
            .HasDefaultValue(VolunteerStatus.Registered);

        builder.HasOne(ev => ev.Event)
            .WithMany(e => e.Volunteers)
            .HasForeignKey(ev => ev.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ev => ev.Volunteer)
            .WithMany(u => u.VolunteerRegistrations)
            .HasForeignKey(ev => ev.VolunteerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ev => new { ev.EventId, ev.VolunteerId }).IsUnique();
    }
}
