using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class AlertGatewayEndpoints
{
    public static WebApplication MapAlertGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/alerts")
            .WithTags("Alerts");

        group.MapGet("/", GetAlerts)
            .WithSummary("Get recent alerts");

        return app;
    }

    private static async Task<IResult> GetAlerts(TransactionsServiceClient client, int take = 100)
    {
        if (take <= 0) take = 100;

        var alerts = await client.Http.GetFromJsonAsync<List<Alert>>($"/internal/alerts?take={take}");
        return Results.Ok(alerts ?? []);
    }
}

