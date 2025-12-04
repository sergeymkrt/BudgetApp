using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Accounts.DTOs;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(string Name, string? Currency = "USD") : IRequest<AccountDto>;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, AccountDto>
{
    private readonly IApplicationDbContext _context;

    public CreateAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
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

        return new AccountDto(account.Id, account.Name, account.Currency);
    }
}
