using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Commands.DeleteBudget;

public record DeleteBudgetCommand(int Id) : IRequest<bool>;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteBudgetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets.FindAsync([request.Id], cancellationToken);

        if (budget is null)
            throw new NotFoundException("Budget", request.Id);

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
