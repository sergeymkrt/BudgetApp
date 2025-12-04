using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Features.Categories.Commands.CreateCategory;
using BudgetApp.Tests.Common;

namespace BudgetApp.Tests.Handlers;

public class CreateCategoryCommandHandlerTests : IDisposable
{
    private readonly BudgetApp.Infrastructure.Data.BudgetDbContext _context;
    private readonly ICacheService _cache;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _context = TestDbContextFactory.Create();
        _cache = new NullCacheService();
        _handler = new CreateCategoryCommandHandler(_context, _cache);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task Handle_ShouldCreateCategory_WithProvidedName()
    {
        // Arrange
        var command = new CreateCategoryCommand("Groceries", "#22c55e");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Groceries");
        result.ColorHex.ShouldBe("#22c55e");
    }

    [Fact]
    public async Task Handle_ShouldPersistCategoryToDatabase()
    {
        // Arrange
        var command = new CreateCategoryCommand("Entertainment", "#6C3483");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var savedCategory = await _context.Categories.FindAsync(result.Id);
        savedCategory.ShouldNotBeNull();
        savedCategory.Name.ShouldBe("Entertainment");
        savedCategory.ColorHex.ShouldBe("#6C3483");
    }

    [Fact]
    public async Task Handle_ShouldAllowNullColorHex()
    {
        // Arrange
        var command = new CreateCategoryCommand("Utilities", null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ColorHex.ShouldBeNull();
    }

    [Fact]
    public async Task Handle_ShouldAutoIncrementId()
    {
        // Arrange
        var command1 = new CreateCategoryCommand("Category 1", null);
        var command2 = new CreateCategoryCommand("Category 2", null);

        // Act
        var result1 = await _handler.Handle(command1, CancellationToken.None);
        var result2 = await _handler.Handle(command2, CancellationToken.None);

        // Assert
        result2.Id.ShouldBeGreaterThan(result1.Id);
    }
}
