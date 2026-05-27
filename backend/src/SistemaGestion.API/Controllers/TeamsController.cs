using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Teams;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/tournaments/{tournamentId:int}/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamRepository _teams;
    private readonly ITournamentRepository _tournaments;

    public TeamsController(ITeamRepository teams, ITournamentRepository tournaments)
    {
        _teams = teams;
        _tournaments = tournaments;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Team>>> GetByTournament(
        int tournamentId,
        CancellationToken cancellationToken)
    {
        if (await _tournaments.GetByIdAsync(tournamentId, cancellationToken) is null)
        {
            return NotFound(new { message = "El torneo no existe." });
        }

        return Ok(await _teams.GetByTournamentAsync(tournamentId, cancellationToken));
    }

    [HttpGet("{id:int}", Name = nameof(GetTeamById))]
    public async Task<ActionResult<Team>> GetTeamById(int tournamentId, int id, CancellationToken cancellationToken)
    {
        var team = await _teams.GetByIdAsync(id, cancellationToken);
        if (team is null || team.TournamentId != tournamentId)
        {
            return NotFound();
        }

        return Ok(team);
    }

    [HttpPost]
    public async Task<ActionResult<Team>> Create(
        int tournamentId,
        [FromBody] CreateTeamRequest request,
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

        var team = new Team
        {
            TournamentId = request.TournamentId,
            Name = request.Name,
            CaptainId = request.CaptainId,
            Points = 0,
            MatchesPlayed = 0
        };

        var created = await _teams.AddAsync(team, cancellationToken);
        return CreatedAtRoute(nameof(GetTeamById), new { tournamentId, id = created.Id }, created);
    }
}
