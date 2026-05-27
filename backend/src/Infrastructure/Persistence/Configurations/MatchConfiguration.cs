using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Persistence.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("matches", t => t.HasCheckConstraint(
            "ck_matches_different_teams",
            "home_team_id <> away_team_id"));

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(m => m.TournamentId)
            .HasColumnName("tournament_id")
            .IsRequired();

        builder.Property(m => m.HomeTeamId)
            .HasColumnName("home_team_id")
            .IsRequired();

        builder.Property(m => m.AwayTeamId)
            .HasColumnName("away_team_id")
            .IsRequired();

        builder.Property(m => m.FieldId)
            .HasColumnName("field_id");

        builder.Property(m => m.ScheduledAt)
            .HasColumnName("scheduled_at")
            .IsRequired();

        builder.Property(m => m.HomeScore)
            .HasColumnName("home_score");

        builder.Property(m => m.AwayScore)
            .HasColumnName("away_score");

        builder.Property(m => m.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(m => m.TournamentId)
            .HasDatabaseName("ix_matches_tournament_id");

        builder.HasOne(m => m.Tournament)
            .WithMany(t => t.Matches)
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Field)
            .WithMany(f => f.Matches)
            .HasForeignKey(m => m.FieldId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
