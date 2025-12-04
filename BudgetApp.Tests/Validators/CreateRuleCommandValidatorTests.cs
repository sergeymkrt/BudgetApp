using BudgetApp.Application.Features.Rules.Commands.CreateRule;
using BudgetApp.Domain.Models;
using FluentValidation.TestHelper;

namespace BudgetApp.Tests.Validators;

public class CreateRuleCommandValidatorTests
{
    private readonly CreateRuleCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_CategoryIdIsZero()
    {
        var command = new CreateRuleCommand(0, "netflix", 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_HaveError_When_PatternIsEmpty()
    {
        var command = new CreateRuleCommand(1, string.Empty, 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Pattern);
    }

    [Fact]
    public void Should_HaveError_When_PatternExceedsMaxLength()
    {
        var command = new CreateRuleCommand(1, new string('a', 201), 1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Pattern);
    }

    [Fact]
    public void Should_HaveError_When_PriorityIsNegative()
    {
        var command = new CreateRuleCommand(1, "netflix", -1, null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Should_NotHaveErrors_When_CommandIsValid()
    {
        var command = new CreateRuleCommand(1, "netflix", 10, TransactionType.Expense);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_AppliesToIsNull()
    {
        var command = new CreateRuleCommand(1, "uber", 5, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_PriorityIsZero()
    {
        var command = new CreateRuleCommand(1, "amazon", 0, null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
