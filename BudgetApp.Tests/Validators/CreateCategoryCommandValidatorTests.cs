using BudgetApp.Application.Features.Categories.Commands.CreateCategory;
using FluentValidation.TestHelper;

namespace BudgetApp.Tests.Validators;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new CreateCategoryCommand(string.Empty, "#FF5733");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaxLength()
    {
        var command = new CreateCategoryCommand(new string('a', 101), "#FF5733");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_ColorHexIsInvalid()
    {
        var command = new CreateCategoryCommand("Groceries", "invalid");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ColorHex);
    }

    [Fact]
    public void Should_HaveError_When_ColorHexIsTooShort()
    {
        var command = new CreateCategoryCommand("Groceries", "#FFF");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ColorHex);
    }

    [Fact]
    public void Should_NotHaveErrors_When_CommandIsValid()
    {
        var command = new CreateCategoryCommand("Entertainment", "#6C3483");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_ColorHexIsNull()
    {
        var command = new CreateCategoryCommand("Transportation", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_ColorHexIsLowercase()
    {
        var command = new CreateCategoryCommand("Food", "#ff5733");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
