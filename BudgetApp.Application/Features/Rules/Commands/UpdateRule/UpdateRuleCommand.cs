using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Rules.Commands.UpdateRule;

public record UpdateRuleCommand(
    int Id,
    int CategoryId,
    string Pattern,
    int Priority,
    TransactionType? AppliesTo
) : IRequest<bool>;

public class UpdateRuleCommandHandler : IRequestHandler<UpdateRuleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _context.CategoryRules.FindAsync([request.Id], cancellationToken);

        if (rule is null)
            throw new NotFoundException("CategoryRule", request.Id);

        rule.CategoryId = request.CategoryId;
        rule.Pattern = request.Pattern;
        rule.Priority = request.Priority;
        rule.AppliesTo = request.AppliesTo;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
