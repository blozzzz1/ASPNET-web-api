namespace UserManagementApi.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedAt);

public record CreateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity);

public record UpdateProductDto(
    string Name,
    string? Description,
    decimal Price,
    int StockQuantity,
    bool IsActive);
