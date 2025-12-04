using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Budgets.Commands.UpdateBudget;

public record UpdateBudgetCommand(
    int Id,
    int CategoryId,
    decimal LimitAmount,
    int Year,
    int Month
) : IRequest<bool>;

public class UpdateBudgetCommandHandler : IRequestHandler<UpdateBudgetCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateBudgetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = await _context.Budgets.FindAsync([request.Id], cancellationToken);

        if (budget is null)
            throw new NotFoundException("Budget", request.Id);

        budget.CategoryId = request.CategoryId;
        budget.LimitAmount = request.LimitAmount;
        budget.Year = request.Year;
        budget.Month = request.Month;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
