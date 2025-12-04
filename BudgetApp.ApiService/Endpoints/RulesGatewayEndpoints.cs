using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class RulesGatewayEndpoints
{
    public static WebApplication MapRulesGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/rules")
            .WithTags("Rules");

        group.MapGet("/", GetRules)
            .WithSummary("Get all categorization rules");

        group.MapPost("/", CreateRule)
            .WithSummary("Create a new rule");

        group.MapPost("/classify", ClassifyTransaction)
            .WithSummary("Classify a transaction using rules");

        return app;
    }

    private static async Task<IResult> GetRules(RulesServiceClient client)
    {
        var rules = await client.Http.GetFromJsonAsync<List<CategoryRule>>("/internal/rules");
        return Results.Ok(rules ?? []);
    }

    private static async Task<IResult> CreateRule(RulesServiceClient client, CategoryRule rule)
    {
        var response = await client.Http.PostAsJsonAsync("/internal/rules", rule);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<CategoryRule>();
        return Results.Created($"/rules/{created?.Id}", created);
    }

    private static async Task<IResult> ClassifyTransaction(
        RulesServiceClient client,
        ClassifyTransactionRequest request)
    {
        var body = new
        {
            request.Description,
            request.Merchant,
            request.Amount,
            request.Type
        };

        var response = await client.Http.PostAsJsonAsync("/internal/rules/classify", body);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<object>();
        return Results.Ok(result);
    }
}

public record ClassifyTransactionRequest(
    string Description,
    string? Merchant,
    decimal Amount,
    TransactionType Type);

