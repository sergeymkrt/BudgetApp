using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand(
    Guid Id,
    Guid AccountId,
    decimal Amount,
    TransactionType Type,
    int? CategoryId,
    DateTimeOffset? Date,
    string Description,
    string? Merchant
) : IRequest<bool>;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FindAsync([request.Id], cancellationToken);

        if (transaction is null)
            throw new NotFoundException("Transaction", request.Id);

        transaction.AccountId = request.AccountId;
        transaction.Amount = request.Amount;
        transaction.Type = request.Type;
        transaction.CategoryId = request.CategoryId;
        transaction.Date = request.Date?.ToUniversalTime() ?? transaction.Date;
        transaction.Description = request.Description;
        transaction.Merchant = request.Merchant;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
