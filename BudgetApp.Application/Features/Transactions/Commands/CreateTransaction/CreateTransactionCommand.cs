using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Transactions.DTOs;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands.CreateTransaction;

/// <summary>
/// Command to create a new transaction.
/// </summary>
public record CreateTransactionCommand(
    Guid AccountId,
    decimal Amount,
    TransactionType Type,
    int? CategoryId,
    DateTimeOffset? Date,
    string Description,
    string? Merchant
) : IRequest<TransactionDto>;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateTransactionCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = request.AccountId,
            Amount = request.Amount,
            Type = request.Type,
            CategoryId = request.CategoryId,
            Date = request.Date?.ToUniversalTime() ?? DateTimeOffset.UtcNow,
            Description = request.Description,
            Merchant = request.Merchant,
            Status = "New"
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate analytics cache for the transaction's month
        _cache.RemoveByPrefix("analytics:");

        return new TransactionDto(
            transaction.Id,
            transaction.AccountId,
            null,
            transaction.Amount,
            transaction.Type,
            transaction.CategoryId,
            null,
            transaction.Date,
            transaction.Description,
            transaction.Merchant,
            transaction.Status);
    }
}
