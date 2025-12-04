using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Transactions.Commands.ApplyCategory;

/// <summary>
/// Command to apply a category to an existing transaction.
/// </summary>
public record ApplyCategoryCommand(
    Guid TransactionId,
    int CategoryId,
    string? Status = null
) : IRequest<bool>;

public class ApplyCategoryCommandHandler : IRequestHandler<ApplyCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ApplyCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApplyCategoryCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions.FindAsync([request.TransactionId], cancellationToken);

        if (transaction is null)
            throw new NotFoundException("Transaction", request.TransactionId);

        transaction.CategoryId = request.CategoryId;

        if (!string.IsNullOrWhiteSpace(request.Status))
            transaction.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
