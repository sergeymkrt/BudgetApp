using FluentValidation;

namespace BudgetApp.Application.Features.Rules.Commands.CreateRule;

public class CreateRuleCommandValidator : AbstractValidator<CreateRuleCommand>
{
    public CreateRuleCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("CategoryId must be greater than zero.");

        RuleFor(x => x.Pattern)
            .NotEmpty().WithMessage("Pattern is required.")
            .MaximumLength(200).WithMessage("Pattern must not exceed 200 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0).WithMessage("Priority must be non-negative.");
    }
}

