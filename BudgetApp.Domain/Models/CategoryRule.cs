namespace BudgetApp.Domain.Models;

public class CategoryRule
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    // e.g. "bolt", "netflix", "uber", "mcdonald"
    public string Pattern { get; set; } = default!;

    // optional: higher = stronger rule
    public int Priority { get; set; } = 0;

    // optional: limit rule to expenses/income
    public TransactionType? AppliesTo { get; set; }
}
