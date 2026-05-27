using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class FieldConfiguration : IEntityTypeConfiguration<Field>
{
    public void Configure(EntityTypeBuilder<Field> builder)
    {
        builder.ToTable("fields");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(f => f.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(f => f.SportType)
            .HasColumnName("sport_type")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(f => f.PricePerHour)
            .HasColumnName("price_per_hour")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(f => f.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true)
            .IsRequired();

        builder.HasMany(f => f.Reservations)
            .WithOne(r => r.Field)
            .HasForeignKey(r => r.FieldId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(f => f.Matches)
            .WithOne(m => m.Field)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
