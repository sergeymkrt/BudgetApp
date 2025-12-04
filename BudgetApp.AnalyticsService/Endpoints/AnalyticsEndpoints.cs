using BudgetApp.Application.Features.Analytics.Queries.GetBudgetStatus;
using BudgetApp.Application.Features.Analytics.Queries.GetMonthlySummary;
using MediatR;

namespace BudgetApp.AnalyticsService.Endpoints;

public static class AnalyticsEndpoints
{
    public static WebApplication MapAnalyticsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/analytics")
            .WithTags("Analytics");

        group.MapGet("/summary", GetMonthlySummary)
            .WithName("GetMonthlySummary")
            .WithSummary("Get monthly income/expense summary");

        group.MapGet("/budget-status", GetBudgetStatus)
            .WithName("GetBudgetStatus")
            .WithSummary("Get budget status by category");

        return app;
    }

    private static async Task<IResult> GetMonthlySummary(
        ISender sender,
        int year,
        int month,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetMonthlySummaryQuery(year, month), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBudgetStatus(
        ISender sender,
        int year,
        int month,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetBudgetStatusQuery(year, month), ct);
        return Results.Ok(result);
    }
}

