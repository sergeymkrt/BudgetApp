namespace BudgetApp.Domain.Models;

public class Budget
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public decimal LimitAmount { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }
}
