using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands.DeleteTransaction;

public record DeleteTransactionCommand(Guid Id) : IRequest<bool>;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTransactionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FindAsync([request.Id], cancellationToken);

        if (transaction is null)
            throw new NotFoundException("Transaction", request.Id);

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
