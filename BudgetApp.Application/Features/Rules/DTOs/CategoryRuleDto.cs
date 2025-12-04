using BudgetApp.Domain.Models;

namespace BudgetApp.Application.Features.Rules.DTOs;

public record CategoryRuleDto(
    int Id,
    int CategoryId,
    string? CategoryName,
    string Pattern,
    int Priority,
    TransactionType? AppliesTo
);

