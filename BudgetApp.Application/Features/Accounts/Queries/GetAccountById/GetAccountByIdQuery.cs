using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Accounts.DTOs;
using MediatR;

namespace BudgetApp.Application.Features.Accounts.Queries.GetAccountById;

public record GetAccountByIdQuery(Guid Id) : IRequest<AccountDto?>;

public class GetAccountByIdQueryHandler : IRequestHandler<GetAccountByIdQuery, AccountDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAccountByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccountDto?> Handle(GetAccountByIdQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FindAsync([request.Id], cancellationToken);
        return account is null ? null : new AccountDto(account.Id, account.Name, account.Currency);
    }
}
