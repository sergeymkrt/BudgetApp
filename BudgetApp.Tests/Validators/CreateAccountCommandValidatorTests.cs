using BudgetApp.Application.Features.Accounts.Commands.CreateAccount;
using FluentValidation.TestHelper;

namespace BudgetApp.Tests.Validators;

public class CreateAccountCommandValidatorTests
{
    private readonly CreateAccountCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        var command = new CreateAccountCommand(string.Empty, "USD");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_NameIsNull()
    {
        var command = new CreateAccountCommand(null!, "USD");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_NameExceedsMaxLength()
    {
        var command = new CreateAccountCommand(new string('a', 101), "USD");

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_CurrencyExceedsMaxLength()
    {
        var command = new CreateAccountCommand("My Account", new string('a', 11));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Currency);
    }

    [Fact]
    public void Should_NotHaveErrors_When_CommandIsValid()
    {
        var command = new CreateAccountCommand("Checking Account", "USD");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_NotHaveErrors_When_CurrencyIsNull()
    {
        var command = new CreateAccountCommand("Savings Account", null);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
