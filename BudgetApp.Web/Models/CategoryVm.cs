namespace BudgetApp.Web.Models;

public class CategoryVm
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? ColorHex { get; set; }
}

