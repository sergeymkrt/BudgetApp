namespace BudgetApp.Web.Models;

public class AlertVm
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public string Type { get; set; } = default!;
    public string Message { get; set; } = default!;
    public int? CategoryId { get; set; }
}
