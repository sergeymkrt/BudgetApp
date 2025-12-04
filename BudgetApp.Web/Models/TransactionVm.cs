namespace BudgetApp.Web.Models;

public class TransactionVm
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public int? CategoryId { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = default!;
    public string? Merchant { get; set; }
    public string Status { get; set; } = default!;
}