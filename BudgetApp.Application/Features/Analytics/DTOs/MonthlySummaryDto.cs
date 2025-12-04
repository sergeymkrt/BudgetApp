namespace BudgetApp.Application.Features.Analytics.DTOs;

public record MonthlySummaryDto(
    int Year,
    int Month,
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal Net
);

