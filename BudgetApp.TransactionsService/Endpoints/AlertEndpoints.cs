using BudgetApp.Application.Features.Alerts.Commands.CreateAlert;
using BudgetApp.Application.Features.Alerts.Queries.GetAlerts;
using MediatR;

namespace BudgetApp.TransactionsService.Endpoints;

public static class AlertEndpoints
{
    public static WebApplication MapAlertEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/alerts")
            .WithTags("Alerts");

        group.MapGet("/", GetAlerts)
            .WithName("GetAlerts")
            .WithSummary("Get recent alerts");

        group.MapPost("/", CreateAlert)
            .WithName("CreateAlert")
            .WithSummary("Create a new alert");

        return app;
    }

    private static async Task<IResult> GetAlerts(
        ISender sender,
        int take = 100,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAlertsQuery(take), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateAlert(
        ISender sender,
        CreateAlertRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateAlertCommand(request.Type, request.Message, request.CategoryId);
        var result = await sender.Send(command, ct);
        return Results.Created($"/internal/alerts/{result.Id}", result);
    }
}

public record CreateAlertRequest(string Type, string Message, int? CategoryId);

