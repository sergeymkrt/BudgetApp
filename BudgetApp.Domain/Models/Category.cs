namespace BudgetApp.Domain.Models;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ColorHex { get; set; }
}
