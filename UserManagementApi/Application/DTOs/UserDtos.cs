namespace UserManagementApi.Application.DTOs;

public record UserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string Role,
    DateTime CreatedAt);

public record CreateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string? Phone);

public record UpdateUserDto(
    string FirstName,
    string LastName,
    string Email,
    string? Phone);
