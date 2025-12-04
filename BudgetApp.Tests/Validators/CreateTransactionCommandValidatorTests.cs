using BudgetApp.Application.Features.Transactions.Commands.CreateTransaction;
using BudgetApp.Domain.Models;
using FluentValidation.TestHelper;

namespace BudgetApp.Tests.Validators;

public class CreateTransactionCommandValidatorTests
{
    private readonly CreateTransactionCommandValidator _validator = new();

    [Fact]
    public void Should_HaveError_When_AccountIdIsEmpty()
    {
        var command = new CreateTransactionCommand(
            Guid.Empty,
            100m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            "Test transaction",
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.AccountId);
    }

    [Fact]
    public void Should_HaveError_When_AmountIsZero()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            0m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            "Test transaction",
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_HaveError_When_AmountIsNegative()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            -50m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            "Test transaction",
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Amount);
    }

    [Fact]
    public void Should_HaveError_When_DescriptionIsEmpty()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            100m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            string.Empty,
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_HaveError_When_DescriptionExceedsMaxLength()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            100m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            new string('a', 501),
            null);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_HaveError_When_MerchantExceedsMaxLength()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            100m,
            TransactionType.Expense,
            null,
            DateTimeOffset.Now,
            "Test transaction",
            new string('a', 201));

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Merchant);
    }

    [Fact]
    public void Should_NotHaveErrors_When_CommandIsValid()
    {
        var command = new CreateTransactionCommand(
            Guid.NewGuid(),
            100m,
            TransactionType.Expense,
            1,
            DateTimeOffset.Now,
            "Grocery shopping",
            "Walmart");

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
