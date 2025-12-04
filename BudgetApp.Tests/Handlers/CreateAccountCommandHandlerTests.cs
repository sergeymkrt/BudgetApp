using BudgetApp.Application.Features.Accounts.Commands.CreateAccount;
using BudgetApp.Tests.Common;

namespace BudgetApp.Tests.Handlers;

public class CreateAccountCommandHandlerTests : IDisposable
{
    private readonly BudgetApp.Infrastructure.Data.BudgetDbContext _context;
    private readonly CreateAccountCommandHandler _handler;

    public CreateAccountCommandHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _handler = new CreateAccountCommandHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ShouldCreateAccount_WithProvidedName()
    {
        // Arrange
        var command = new CreateAccountCommand("Checking Account", "USD");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Checking Account");
        result.Currency.ShouldBe("USD");
        result.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ShouldPersistAccountToDatabase()
    {
        // Arrange
        var command = new CreateAccountCommand("Savings Account", "EUR");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var savedAccount = await _context.Accounts.FindAsync(result.Id);
        savedAccount.ShouldNotBeNull();
        savedAccount.Name.ShouldBe("Savings Account");
        savedAccount.Currency.ShouldBe("EUR");
    }

    [Fact]
    public async Task Handle_ShouldDefaultToUSD_WhenCurrencyIsNull()
    {
        // Arrange
        var command = new CreateAccountCommand("Account", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Currency.ShouldBe("USD");
    }

    [Fact]
    public async Task Handle_ShouldDefaultToUSD_WhenCurrencyIsEmpty()
    {
        // Arrange
        var command = new CreateAccountCommand("Account", "");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Currency.ShouldBe("USD");
    }

    [Fact]
    public async Task Handle_ShouldGenerateUniqueIds()
    {
        // Arrange
        var command1 = new CreateAccountCommand("Account 1", "USD");
        var command2 = new CreateAccountCommand("Account 2", "USD");

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        result1.Id.ShouldNotBe(result2.Id);
    }
}
