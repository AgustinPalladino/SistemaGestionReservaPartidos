using Domain.Enums;

namespace SistemaGestion.API.Models.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    UserRole Role);
