using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Rules.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Rules.Queries.GetRuleById;

public record GetRuleByIdQuery(int Id) : IRequest<CategoryRuleDto?>;

public class GetRuleByIdQueryHandler : IRequestHandler<GetRuleByIdQuery, CategoryRuleDto?>
{
    private readonly IApplicationDbContext _context;

    public GetRuleByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryRuleDto?> Handle(
        GetRuleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var rule = await _context.CategoryRules
            .Include(r => r.Category)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

        return rule is null ? null : new CategoryRuleDto(
            rule.Id,
            rule.CategoryId,
            rule.Category?.Name,
            rule.Pattern,
            rule.Priority,
            rule.AppliesTo);
    }
}
