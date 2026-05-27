using Application.Interfaces;
using Application.Interfaces.Persistence.Repositories;
using Application.Services.Matches;
using Application.Services.Reservations;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Matches;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/tournaments/{tournamentId:int}/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly IMatchRepository _matches;
    private readonly ITournamentRepository _tournaments;
    private readonly ITeamRepository _teams;
    private readonly IFieldRepository _fields;
    private readonly IReservationScheduleService _scheduleService;

    public MatchesController(
        IMatchRepository matches,
        ITournamentRepository tournaments,
        ITeamRepository teams,
        IFieldRepository fields,
        IReservationScheduleService scheduleService)
    {
        _matches = matches;
        _tournaments = tournaments;
        _teams = teams;
        _fields = fields;
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Match>>> GetByTournament(
        int tournamentId,
        CancellationToken cancellationToken)
    {
        if (await _tournaments.GetByIdAsync(tournamentId, cancellationToken) is null)
        {
            return NotFound(new { message = "El torneo no existe." });
        }

        return Ok(await _matches.GetByTournamentAsync(tournamentId, cancellationToken));
    }

    [HttpGet("{id:int}", Name = nameof(GetMatchById))]
    public async Task<ActionResult<Match>> GetMatchById(int tournamentId, int id, CancellationToken cancellationToken)
    {
        var match = await _matches.GetByIdAsync(id, cancellationToken);
        if (match is null || match.TournamentId != tournamentId)
        {
            return NotFound();
        }

        return Ok(match);
    }

    [HttpPost]
    public async Task<ActionResult<Match>> Create(
        int tournamentId,
        [FromBody] CreateMatchRequest request,
        CancellationToken cancellationToken)
    {
        if (tournamentId != request.TournamentId)
        {
            return BadRequest(new { message = "El torneo de la ruta no coincide con el del cuerpo." });
        }

        if (await _tournaments.GetByIdAsync(tournamentId, cancellationToken) is null)
        {
            return NotFound(new { message = "El torneo no existe." });
        }

        try
        {
            MatchValidation.EnsureDifferentTeams(request.HomeTeamId, request.AwayTeamId);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var homeTeam = await _teams.GetByIdAsync(request.HomeTeamId, cancellationToken);
        var awayTeam = await _teams.GetByIdAsync(request.AwayTeamId, cancellationToken);
        if (homeTeam?.TournamentId != tournamentId || awayTeam?.TournamentId != tournamentId)
        {
            return BadRequest(new { message = "Los equipos deben pertenecer al torneo." });
        }

        if (request.FieldId is int fieldId)
        {
            if (await _fields.GetByIdAsync(fieldId, cancellationToken) is null)
            {
                return BadRequest(new { message = "La cancha no existe." });
            }

            var date = DateOnly.FromDateTime(request.ScheduledAt);
            var start = TimeOnly.FromDateTime(request.ScheduledAt);
            var end = start.Add(TimeSpan.FromMinutes(ScheduleConflictOptions.DefaultMatchDurationMinutes));

            if (await _scheduleService.HasScheduleConflictAsync(
                    fieldId, date, start, end, cancellationToken))
            {
                return Conflict(new { message = "La cancha ya está ocupada en ese horario." });
            }
        }

        var match = new Match
        {
            TournamentId = request.TournamentId,
            HomeTeamId = request.HomeTeamId,
            AwayTeamId = request.AwayTeamId,
            FieldId = request.FieldId,
            ScheduledAt = request.ScheduledAt,
            Status = request.Status
        };

        var created = await _matches.AddAsync(match, cancellationToken);
        return CreatedAtRoute(nameof(GetMatchById), new { tournamentId, id = created.Id }, created);
    }

    [HttpPatch("{id:int}/score")]
    public async Task<IActionResult> UpdateScore(
        int tournamentId,
        int id,
        [FromBody] UpdateMatchScoreRequest request,
        CancellationToken cancellationToken)
    {
        var match = await _matches.GetByIdAsync(id, cancellationToken);
        if (match is null || match.TournamentId != tournamentId)
        {
            return NotFound();
        }

        match.HomeScore = request.HomeScore;
        match.AwayScore = request.AwayScore;
        match.Status = request.Status;

        await _matches.UpdateAsync(match, cancellationToken);
        return NoContent();
    }
}
