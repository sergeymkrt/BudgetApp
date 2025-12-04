namespace BudgetApp.Web.Models;

public class BudgetVm
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal LimitAmount { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}

