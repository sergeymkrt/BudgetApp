namespace BudgetApp.Application.Features.Alerts.DTOs;

public record AlertDto(
    Guid Id,
    DateTimeOffset CreatedAt,
    string Type,
    string Message,
    int? CategoryId,
    string? CategoryName
);

