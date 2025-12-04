using BudgetApp.Application.Features.Budgets.Commands.CreateBudget;
using FluentValidation.TestHelper;

namespace BudgetApp.Tests.Validators;

public class CreateBudgetCommandValidatorTests
{
    private readonly CreateBudgetCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_CategoryIdIsZero()
    {
        var command = new CreateBudgetCommand(0, 500m, 2025, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_HaveError_When_CategoryIdIsNegative()
    {
        var command = new CreateBudgetCommand(-1, 500m, 2025, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Should_HaveError_When_LimitAmountIsZero()
    {
        var command = new CreateBudgetCommand(1, 0m, 2025, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LimitAmount);
    }

    [Fact]
    public void Should_HaveError_When_LimitAmountIsNegative()
    {
        var command = new CreateBudgetCommand(1, -100m, 2025, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LimitAmount);
    }

    [Fact]
    public void Should_HaveError_When_YearIsTooLow()
    {
        var command = new CreateBudgetCommand(1, 500m, 1999, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Should_HaveError_When_YearIsTooHigh()
    {
        var command = new CreateBudgetCommand(1, 500m, 2101, 1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Fact]
    public void Should_HaveError_When_MonthIsZero()
    {
        var command = new CreateBudgetCommand(1, 500m, 2025, 0);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void Should_HaveError_When_MonthIsGreaterThan12()
    {
        var command = new CreateBudgetCommand(1, 500m, 2025, 13);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Month);
    }

    [Fact]
    public void Should_NotHaveErrors_When_CommandIsValid()
    {
        var command = new CreateBudgetCommand(1, 500m, 2025, 6);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(6)]
    [InlineData(12)]
    public void Should_NotHaveErrors_For_ValidMonths(int month)
    {
        var command = new CreateBudgetCommand(1, 500m, 2025, month);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Month);
    }
}
