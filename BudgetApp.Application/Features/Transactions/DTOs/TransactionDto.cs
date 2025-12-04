using BudgetApp.Domain.Models;

namespace BudgetApp.Application.Features.Transactions.DTOs;

public record TransactionDto(
    Guid Id,
    Guid AccountId,
    string? AccountName,
    decimal Amount,
    TransactionType Type,
    int? CategoryId,
    string? CategoryName,
    DateTimeOffset Date,
    string Description,
    string? Merchant,
    string Status
);

