using Application.Interfaces;
using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Reservations;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationRepository _reservations;
    private readonly IFieldRepository _fields;
    private readonly IUserRepository _users;
    private readonly IReservationScheduleService _scheduleService;

    public ReservationsController(
        IReservationRepository reservations,
        IFieldRepository fields,
        IUserRepository users,
        IReservationScheduleService scheduleService)
    {
        _reservations = reservations;
        _fields = fields;
        _users = users;
        _scheduleService = scheduleService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Reservation>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _reservations.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Reservation>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservations.GetByIdAsync(id, cancellationToken);
        return reservation is null ? NotFound() : Ok(reservation);
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> Create(
        [FromBody] CreateReservationRequest request,
        CancellationToken cancellationToken)
    {
        if (await _fields.GetByIdAsync(request.FieldId, cancellationToken) is null)
        {
            return BadRequest(new { message = "La cancha no existe." });
        }

        if (await _users.GetByIdAsync(request.UserId, cancellationToken) is null)
        {
            return BadRequest(new { message = "El usuario no existe." });
        }

        if (await _scheduleService.HasScheduleConflictAsync(
                request.FieldId,
                request.Date,
                request.StartTime,
                request.EndTime,
                cancellationToken))
        {
            return Conflict(new { message = "La cancha ya está ocupada en ese horario." });
        }

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FieldId = request.FieldId,
            Date = request.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Status = ReservationStatus.Pending,
            TotalAmount = request.TotalAmount,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reservations.AddAsync(reservation, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservations.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return NotFound();
        }

        reservation.Status = ReservationStatus.Confirmed;
        await _reservations.UpdateAsync(reservation, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var reservation = await _reservations.GetByIdAsync(id, cancellationToken);
        if (reservation is null)
        {
            return NotFound();
        }

        reservation.Status = ReservationStatus.Cancelled;
        await _reservations.UpdateAsync(reservation, cancellationToken);
        return NoContent();
    }
}
