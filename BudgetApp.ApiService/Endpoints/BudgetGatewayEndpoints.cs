using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class BudgetGatewayEndpoints
{
    public static WebApplication MapBudgetGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/budgets")
            .WithTags("Budgets");

        group.MapGet("/", GetBudgets)
            .WithSummary("Get all budgets");

        group.MapGet("/{id:int}", GetBudgetById)
            .WithSummary("Get a budget by ID");

        group.MapPost("/", CreateBudget)
            .WithSummary("Create a new budget");

        group.MapPut("/{id:int}", UpdateBudget)
            .WithSummary("Update a budget");

        group.MapDelete("/{id:int}", DeleteBudget)
            .WithSummary("Delete a budget");

        return app;
    }

    private static async Task<IResult> GetBudgets(TransactionsServiceClient client, int? year, int? month)
    {
        var query = new List<string>();
        if (year.HasValue) query.Add($"year={year.Value}");
        if (month.HasValue) query.Add($"month={month.Value}");
        var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

        var budgets = await client.Http.GetFromJsonAsync<object>($"/internal/budgets{qs}");
        return Results.Ok(budgets);
    }

    private static async Task<IResult> GetBudgetById(TransactionsServiceClient client, int id)
    {
        var response = await client.Http.GetAsync($"/internal/budgets/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        var budget = await response.Content.ReadFromJsonAsync<Budget>();
        return Results.Ok(budget);
    }

    private static async Task<IResult> CreateBudget(TransactionsServiceClient client, Budget budget)
    {
        var response = await client.Http.PostAsJsonAsync("/internal/budgets", budget);

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.BadRequest(error);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<Budget>();
        return Results.Created($"/budgets/{created?.Id}", created);
    }

    private static async Task<IResult> UpdateBudget(TransactionsServiceClient client, int id, Budget budget)
    {
        var response = await client.Http.PutAsJsonAsync($"/internal/budgets/{id}", budget);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteBudget(TransactionsServiceClient client, int id)
    {
        var response = await client.Http.DeleteAsync($"/internal/budgets/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        return Results.NoContent();
    }
}

