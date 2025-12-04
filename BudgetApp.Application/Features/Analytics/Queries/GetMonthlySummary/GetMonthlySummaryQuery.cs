using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Analytics.DTOs;
using BudgetApp.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Analytics.Queries.GetMonthlySummary;

public record GetMonthlySummaryQuery(int Year, int Month) : IRequest<MonthlySummaryDto>;

public class GetMonthlySummaryQueryHandler : IRequestHandler<GetMonthlySummaryQuery, MonthlySummaryDto>
{
    private readonly IApplicationDbContext _context;

    public GetMonthlySummaryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MonthlySummaryDto> Handle(
        GetMonthlySummaryQuery request,
        CancellationToken cancellationToken)
    {
        var from = new DateTimeOffset(request.Year, request.Month, 1, 0, 0, 0, TimeSpan.Zero);
        var to = from.AddMonths(1);

        var transactions = await _context.Transactions
            .Where(t => t.Date >= from && t.Date < to)
            .ToListAsync(cancellationToken);

        var totalIncome = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpenses = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        return new MonthlySummaryDto(
            request.Year,
            request.Month,
            totalIncome,
            totalExpenses,
            totalIncome - totalExpenses);
    }
}
