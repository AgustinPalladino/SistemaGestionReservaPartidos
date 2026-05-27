using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id");

        builder.Property(r => r.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(r => r.FieldId)
            .HasColumnName("field_id")
            .IsRequired();

        builder.Property(r => r.Date)
            .HasColumnName("date")
            .IsRequired();

        builder.Property(r => r.StartTime)
            .HasColumnName("start_time")
            .IsRequired();

        builder.Property(r => r.EndTime)
            .HasColumnName("end_time")
            .IsRequired();

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.TotalAmount)
            .HasColumnName("total_amount")
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(r => new { r.FieldId, r.Date })
            .HasDatabaseName("ix_reservations_field_id_date");

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Field)
            .WithMany(f => f.Reservations)
            .HasForeignKey(r => r.FieldId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
