using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.TournamentId)
            .HasColumnName("tournament_id")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.CaptainId)
            .HasColumnName("captain_id");

        builder.Property(t => t.Points)
            .HasColumnName("points")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(t => t.MatchesPlayed)
            .HasColumnName("matches_played")
            .HasDefaultValue(0)
            .IsRequired();

        builder.HasOne(t => t.Tournament)
            .WithMany(tournament => tournament.Teams)
            .HasForeignKey(t => t.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Captain)
            .WithMany(u => u.CaptainedTeams)
            .HasForeignKey(t => t.CaptainId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
