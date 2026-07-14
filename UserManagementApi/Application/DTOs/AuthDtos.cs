namespace UserManagementApi.Application.DTOs;

public record LoginDto(string Email, string Password);

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone);

public record AuthUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    DateTime CreatedAt);
