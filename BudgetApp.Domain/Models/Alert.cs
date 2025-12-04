namespace BudgetApp.Domain.Models;

public class Alert
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string Type { get; set; } = default!;

    public string Message { get; set; } = default!;

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
