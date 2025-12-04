using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Transactions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Transactions.Queries.GetTransactions;

/// <summary>
/// Query to retrieve recent transactions.
/// </summary>
public record GetTransactionsQuery(int Take = 50) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .Take(request.Take)
            .Select(t => new TransactionDto(
                t.Id,
                t.AccountId,
                t.Account != null ? t.Account.Name : null,
                t.Amount,
                t.Type,
                t.CategoryId,
                t.Category != null ? t.Category.Name : null,
                t.Date,
                t.Description,
                t.Merchant,
                t.Status))
            .ToListAsync(cancellationToken);

        return transactions;
    }
}
