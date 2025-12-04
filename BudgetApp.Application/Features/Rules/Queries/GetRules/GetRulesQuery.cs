using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Rules.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Rules.Queries.GetRules;

public record GetRulesQuery : IRequest<IReadOnlyList<CategoryRuleDto>>;

public class GetRulesQueryHandler : IRequestHandler<GetRulesQuery, IReadOnlyList<CategoryRuleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetRulesQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IReadOnlyList<CategoryRuleDto>> Handle(
        GetRulesQuery request,
        CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.AllRules,
            async () => await _context.CategoryRules
                .Include(r => r.Category)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.Pattern)
                .Select(r => new CategoryRuleDto(
                    r.Id,
                    r.CategoryId,
                    r.Category != null ? r.Category.Name : null,
                    r.Pattern,
                    r.Priority,
                    r.AppliesTo))
                .ToListAsync(cancellationToken),
            TimeSpan.FromMinutes(10));
    }
}
