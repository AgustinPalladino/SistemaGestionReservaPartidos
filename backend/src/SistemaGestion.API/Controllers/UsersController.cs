using Application.Interfaces.Persistence.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using SistemaGestion.API.Models.Users;

namespace SistemaGestion.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _users;

    public UsersController(IUserRepository users) => _users = users;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await _users.GetAllAsync(cancellationToken);
        return Ok(users.Select(ToPublicResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<object>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        return user is null ? NotFound() : Ok(ToPublicResponse(user));
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        if (await _users.GetByEmailAsync(request.Email, cancellationToken) is not null)
        {
            return Conflict(new { message = "El email ya está registrado." });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _users.AddAsync(user, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToPublicResponse(created));
    }

    private static object ToPublicResponse(User user) => new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.Role,
        user.CreatedAt
    };
}
