using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Accounts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Accounts.Queries.GetAccounts;

public record GetAccountsQuery : IRequest<IReadOnlyList<AccountDto>>;

public class GetAccountsQueryHandler : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public GetAccountsQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IReadOnlyList<AccountDto>> Handle(
        GetAccountsQuery request,
        CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(
            CacheKeys.AllAccounts,
            async () => await _context.Accounts
                .OrderBy(a => a.Name)
                .Select(a => new AccountDto(a.Id, a.Name, a.Currency))
                .ToListAsync(cancellationToken),
            TimeSpan.FromMinutes(10));
    }
}
