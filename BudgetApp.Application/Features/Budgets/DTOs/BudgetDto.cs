namespace BudgetApp.Application.Features.Budgets.DTOs;

public record BudgetDto(
    int Id,
    int CategoryId,
    string? CategoryName,
    decimal LimitAmount,
    int Year,
    int Month
);

