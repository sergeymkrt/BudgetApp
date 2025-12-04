using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Analytics.DTOs;
using BudgetApp.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Analytics.Queries.GetBudgetStatus;

public record GetBudgetStatusQuery(int Year, int Month) : IRequest<IReadOnlyList<BudgetStatusDto>>;

public class GetBudgetStatusQueryHandler
    : IRequestHandler<GetBudgetStatusQuery, IReadOnlyList<BudgetStatusDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BudgetStatusDto>> Handle(
        GetBudgetStatusQuery request,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var to = from.AddMonths(1);

        var transactions = await _context.Transactions
            .Where(t => t.Date >= from && t.Date < to)
            .ToListAsync(cancellationToken);

        var budgets = await _context.Budgets
            .Include(b => b.Category)
            .ToListAsync(cancellationToken);

        var spendByCategory = transactions
            .Where(t => t.Type == TransactionType.Expense && t.CategoryId != null)
            .GroupBy(t => t.CategoryId)
            .Select(g => new { CategoryId = g.Key!.Value, Spent = g.Sum(x => x.Amount) })
            .ToList();

        var result = (
            from b in budgets
            join s in spendByCategory on b.CategoryId equals s.CategoryId into sg
            from s in sg.DefaultIfEmpty()
            select new BudgetStatusDto(
                b.CategoryId,
                b.Category?.Name ?? "Unknown",
                b.LimitAmount,
                s?.Spent ?? 0,
                (s?.Spent ?? 0) > b.LimitAmount
            )).ToList();

        return result;
    }
}
