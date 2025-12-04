using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid Id) : IRequest<Result>;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public DeleteAccountCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FindAsync([request.Id], cancellationToken);

        if (account is null)
            throw new NotFoundException("Account", request.Id);

        var hasTransactions = await _context.Transactions
            .AnyAsync(t => t.AccountId == request.Id, cancellationToken);

        if (hasTransactions)
            return Result.Failure("Cannot delete account with existing transactions.");

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove(CacheKeys.AllAccounts);

        return Result.Success();
    }
}
