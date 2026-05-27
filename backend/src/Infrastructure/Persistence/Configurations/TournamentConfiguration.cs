using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Persistence.Configurations;

public class TournamentConfiguration : IEntityTypeConfiguration<Tournament>
{
    public void Configure(EntityTypeBuilder<Tournament> builder)
    {
        builder.ToTable("tournaments");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.OrganizerId)
            .HasColumnName("organizer_id")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.SportType)
            .HasColumnName("sport_type")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.StartDate)
            .HasColumnName("start_date")
            .IsRequired();

        builder.Property(t => t.EndDate)
            .HasColumnName("end_date")
            .IsRequired();

        builder.HasOne(t => t.Organizer)
            .WithMany(u => u.OrganizedTournaments)
            .HasForeignKey(t => t.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Teams)
            .WithOne(team => team.Tournament)
            .HasForeignKey(team => team.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Matches)
            .WithOne(m => m.Tournament)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
