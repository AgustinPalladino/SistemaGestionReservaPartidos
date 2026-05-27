using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Fields;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldsController : ControllerBase
{
    private readonly IFieldRepository _fields;

    public FieldsController(IFieldRepository fields) => _fields = fields;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Field>>> GetAll(
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
        => Ok(await _fields.GetAllAsync(activeOnly, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Field>> GetById(int id, CancellationToken cancellationToken)
    {
        var field = await _fields.GetByIdAsync(id, cancellationToken);
        return field is null ? NotFound() : Ok(field);
    }

    [HttpPost]
    public async Task<ActionResult<Field>> Create(
        [FromBody] CreateFieldRequest request,
        CancellationToken cancellationToken)
    {
        var field = new Field
        {
            Name = request.Name,
            SportType = request.SportType,
            PricePerHour = request.PricePerHour,
            IsActive = true
        };

        var created = await _fields.AddAsync(field, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateFieldRequest request,
        CancellationToken cancellationToken)
    {
        var field = await _fields.GetByIdAsync(id, cancellationToken);
        if (field is null)
        {
            return NotFound();
        }

        field.Name = request.Name;
        field.SportType = request.SportType;
        field.PricePerHour = request.PricePerHour;
        field.IsActive = request.IsActive;

        await _fields.UpdateAsync(field, cancellationToken);
        return NoContent();
    }
}
