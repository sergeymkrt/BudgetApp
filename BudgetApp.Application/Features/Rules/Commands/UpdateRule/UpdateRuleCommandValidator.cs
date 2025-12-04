using FluentValidation;

namespace BudgetApp.Application.Features.Rules.Commands.UpdateRule;

public class UpdateRuleCommandValidator : AbstractValidator<UpdateRuleCommand>
{
    public UpdateRuleCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Rule ID is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category is required.");

        RuleFor(x => x.Pattern)
            .NotEmpty()
            .WithMessage("Pattern is required.")
            .MaximumLength(200)
            .WithMessage("Pattern must not exceed 200 characters.");

        RuleFor(x => x.Priority)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Priority must be zero or positive.");
    }
}


