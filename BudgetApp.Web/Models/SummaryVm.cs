namespace BudgetApp.Web.Models;

public class SummaryVm
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal Net { get; set; }
}