using FluentValidation;

namespace BudgetApp.Application.Features.Accounts.Commands.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Account ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Account name is required.")
            .MaximumLength(100)
            .WithMessage("Account name must not exceed 100 characters.");

        RuleFor(x => x.Currency)
            .MaximumLength(10)
            .WithMessage("Currency code must not exceed 10 characters.");
    }
}


