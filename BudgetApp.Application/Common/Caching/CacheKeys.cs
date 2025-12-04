namespace BudgetApp.Application.Common.Caching;

/// <summary>
/// Centralized cache key definitions for consistency.
/// </summary>
public static class CacheKeys
{
    public const string AllCategories = "categories:all";
    public const string AllAccounts = "accounts:all";
    public const string AllRules = "rules:all";
    
    public static string MonthlySummary(int year, int month) => $"analytics:summary:{year}:{month}";
    public static string BudgetStatus(int year, int month) => $"analytics:budget-status:{year}:{month}";
}

