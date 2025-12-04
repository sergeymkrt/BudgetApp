using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Rules.Commands.DeleteRule;

public record DeleteRuleCommand(int Id) : IRequest<bool>;

public class DeleteRuleCommandHandler : IRequestHandler<DeleteRuleCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteRuleCommand request, CancellationToken cancellationToken)
    {
        var rule = await _context.CategoryRules.FindAsync([request.Id], cancellationToken);

        if (rule is null)
            throw new NotFoundException("CategoryRule", request.Id);

        _context.CategoryRules.Remove(rule);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
