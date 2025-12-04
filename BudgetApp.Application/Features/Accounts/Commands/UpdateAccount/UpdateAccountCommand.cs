using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Accounts.Commands.UpdateAccount;

public record UpdateAccountCommand(Guid Id, string Name, string? Currency) : IRequest<bool>;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateAccountCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FindAsync([request.Id], cancellationToken);

        if (account is null)
            throw new NotFoundException("Account", request.Id);

        account.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.Currency))
            account.Currency = request.Currency;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
