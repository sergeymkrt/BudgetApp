using FluentValidation;

namespace BudgetApp.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ColorHex)
            .MaximumLength(7).WithMessage("ColorHex must not exceed 7 characters.")
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .When(x => !string.IsNullOrEmpty(x.ColorHex))
            .WithMessage("ColorHex must be a valid hex color (e.g., #FF5733).");
    }
}

