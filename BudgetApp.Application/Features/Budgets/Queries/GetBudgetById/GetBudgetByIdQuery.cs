using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Budgets.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Budgets.Queries.GetBudgetById;

public record GetBudgetByIdQuery(int Id) : IRequest<BudgetDto?>;

public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, BudgetDto?>
{
    private readonly IApplicationDbContext _context;

    public GetBudgetByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetDto?> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        return budget is null ? null : new BudgetDto(
            budget.Id,
            budget.CategoryId,
            budget.Category?.Name,
            budget.LimitAmount,
            budget.Year,
            budget.Month);
    }
}
