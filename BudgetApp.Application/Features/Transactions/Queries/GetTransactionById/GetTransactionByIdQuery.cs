using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Transactions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<TransactionDto?>;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionDto?>
{
    private readonly IApplicationDbContext _context;

    public GetTransactionByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TransactionDto?> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (transaction is null)
            return null;

        return new TransactionDto(
            transaction.Id,
            transaction.AccountId,
            transaction.Account?.Name,
            transaction.Amount,
            transaction.Type,
            transaction.CategoryId,
            transaction.Category?.Name,
            transaction.Date,
            transaction.Description,
            transaction.Merchant,
            transaction.Status);
    }
}
