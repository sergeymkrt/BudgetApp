using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Transactions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Transactions.Queries.GetUncategorizedTransactions;

/// <summary>
/// Query to retrieve transactions that haven't been categorized yet.
/// </summary>
public record GetUncategorizedTransactionsQuery(int Take = 50) : IRequest<IReadOnlyList<TransactionDto>>;

public class GetUncategorizedTransactionsQueryHandler
    : IRequestHandler<GetUncategorizedTransactionsQuery, IReadOnlyList<TransactionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUncategorizedTransactionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<TransactionDto>> Handle(
        GetUncategorizedTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        var transactions = await _context.Transactions
            .Where(t => t.Status == "New" || t.CategoryId == null)
            .OrderBy(t => t.Date)
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
