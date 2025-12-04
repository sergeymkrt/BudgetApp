using BudgetApp.ApiService.Services;

namespace BudgetApp.ApiService.Endpoints;

public static class AnalyticsGatewayEndpoints
{
    public static WebApplication MapAnalyticsGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/analytics")
            .WithTags("Analytics");

        group.MapGet("/summary", GetMonthlySummary)
            .WithSummary("Get monthly income/expense summary");

        group.MapGet("/budget-status", GetBudgetStatus)
            .WithSummary("Get budget status by category");

        return app;
    }

    private static async Task<IResult> GetMonthlySummary(AnalyticsServiceClient client, int year, int month)
    {
        var url = $"/internal/analytics/summary?year={year}&month={month}";
        var result = await client.Http.GetFromJsonAsync<object>(url);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBudgetStatus(AnalyticsServiceClient client, int year, int month)
    {
        var url = $"/internal/analytics/budget-status?year={year}&month={month}";
        var result = await client.Http.GetFromJsonAsync<object>(url);
        return Results.Ok(result);
    }
}

