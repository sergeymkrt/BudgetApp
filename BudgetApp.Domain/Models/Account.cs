namespace BudgetApp.Domain.Models;
public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Currency { get; set; } = "USD";
}
