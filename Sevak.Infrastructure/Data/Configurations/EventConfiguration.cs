using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;

namespace Sevak.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasDefaultValue(EventStatus.Draft);

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(2000);
        builder.Property(e => e.Location).HasMaxLength(300).IsRequired();

        builder.HasOne(e => e.Organizer)
            .WithMany(u => u.OrganizedEvents)
            .HasForeignKey(e => e.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EventDate);
        builder.HasIndex(e => e.Status);
    }
}
