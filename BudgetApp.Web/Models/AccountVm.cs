namespace BudgetApp.Web.Models;

public class AccountVm
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Currency { get; set; } = default!;
}
