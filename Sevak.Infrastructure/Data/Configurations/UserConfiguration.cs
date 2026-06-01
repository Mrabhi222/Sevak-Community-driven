using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sevak.Domain.Entities;
using Sevak.Domain.Enums;

namespace Sevak.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasDefaultValue(UserRole.Volunteer);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Skills)
            .HasColumnType("jsonb")
            .HasConversion(
                v => v == null ? "[]" : System.Text.Json.JsonSerializer.Serialize(v),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v) ?? new List<string>());
    }
}
