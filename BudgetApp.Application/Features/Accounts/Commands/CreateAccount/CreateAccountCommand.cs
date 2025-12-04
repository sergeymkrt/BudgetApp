using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Accounts.DTOs;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(string Name, string? Currency = "USD") : IRequest<AccountDto>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateAccountCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<AccountDto> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "USD" : request.Currency
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove(CacheKeys.AllAccounts);

        return new AccountDto(account.Id, account.Name, account.Currency);
    }
}
