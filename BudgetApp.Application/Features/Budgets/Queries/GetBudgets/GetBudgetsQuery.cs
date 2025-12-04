using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Budgets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Budgets.Queries.GetBudgets;

public record GetBudgetsQuery(int? Year = null, int? Month = null) : IRequest<IReadOnlyList<BudgetDto>>;

public class GetBudgetsQueryHandler : IRequestHandler<GetBudgetsQuery, IReadOnlyList<BudgetDto>>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<BudgetDto>> Handle(
        GetBudgetsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Budgets.Include(b => b.Category).AsQueryable();

        if (request.Year.HasValue)
            query = query.Where(b => b.Year == request.Year.Value);

        if (request.Month.HasValue)
            query = query.Where(b => b.Month == request.Month.Value);

        return await query
            .OrderBy(b => b.Category!.Name)
            .Select(b => new BudgetDto(
                b.Id,
                b.CategoryId,
                b.Category != null ? b.Category.Name : null,
                b.LimitAmount,
                b.Year,
                b.Month))
            .ToListAsync(cancellationToken);
    }
}
