namespace BudgetApp.Application.Features.Analytics.DTOs;

public record BudgetStatusDto(
    int CategoryId,
    string CategoryName,
    decimal LimitAmount,
    decimal Spent,
    bool IsOver
);

