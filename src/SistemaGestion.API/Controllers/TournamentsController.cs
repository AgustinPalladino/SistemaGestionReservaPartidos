using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Tournaments;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentRepository _tournaments;
    private readonly IUserRepository _users;

    public TournamentsController(ITournamentRepository tournaments, IUserRepository users)
    {
        _tournaments = tournaments;
        _users = users;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Tournament>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _tournaments.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Tournament>> GetById(int id, CancellationToken cancellationToken)
    {
        var tournament = await _tournaments.GetByIdAsync(id, cancellationToken);
        return tournament is null ? NotFound() : Ok(tournament);
    }

    [HttpPost]
    public async Task<ActionResult<Tournament>> Create(
        [FromBody] CreateTournamentRequest request,
        CancellationToken cancellationToken)
    {
        if (await _users.GetByIdAsync(request.OrganizerId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El organizador no existe." });
        }

        if (request.EndDate < request.StartDate)
        {
            return BadRequest(new { message = "La fecha de fin no puede ser anterior a la de inicio." });
        }

        var tournament = new Tournament
        {
            OrganizerId = request.OrganizerId,
            Name = request.Name,
            SportType = request.SportType,
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        var created = await _tournaments.AddAsync(tournament, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateTournamentRequest request,
        CancellationToken cancellationToken)
    {
        var tournament = await _tournaments.GetByIdAsync(id, cancellationToken);
        if (tournament is null)
        {
            return NotFound();
        }

        if (request.EndDate < request.StartDate)
        {
            return BadRequest(new { message = "La fecha de fin no puede ser anterior a la de inicio." });
        }

        tournament.Name = request.Name;
        tournament.SportType = request.SportType;
        tournament.Status = request.Status;
        tournament.StartDate = request.StartDate;
        tournament.EndDate = request.EndDate;

        await _tournaments.UpdateAsync(tournament, cancellationToken);
        return NoContent();
    }
}
