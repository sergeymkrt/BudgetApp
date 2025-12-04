using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class TransactionGatewayEndpoints
{
    public static WebApplication MapTransactionGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/transactions")
            .WithTags("Transactions");

        group.MapGet("/", GetTransactions)
            .WithSummary("Get recent transactions");

        group.MapGet("/{id:guid}", GetTransactionById)
            .WithSummary("Get a transaction by ID");

        group.MapPost("/", CreateTransaction)
            .WithSummary("Create a new transaction");

        group.MapPut("/{id:guid}", UpdateTransaction)
            .WithSummary("Update a transaction");

        group.MapDelete("/{id:guid}", DeleteTransaction)
            .WithSummary("Delete a transaction");

        return app;
    }

    private static async Task<IResult> GetTransactions(TransactionsServiceClient client)
    {
        var result = await client.Http.GetFromJsonAsync<List<object>>("/internal/transactions");
        return Results.Ok(result ?? []);
    }

    private static async Task<IResult> GetTransactionById(TransactionsServiceClient client, Guid id)
    {
        var response = await client.Http.GetAsync($"/internal/transactions/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        var transaction = await response.Content.ReadFromJsonAsync<Transaction>();
        return Results.Ok(transaction);
    }

    private static async Task<IResult> CreateTransaction(
        TransactionsServiceClient client,
        Transaction tx)
    {
        var response = await client.Http.PostAsJsonAsync("/internal/transactions", tx);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<Transaction>();
        return Results.Created($"/transactions/{created?.Id}", created);
    }

    private static async Task<IResult> UpdateTransaction(
        TransactionsServiceClient client,
        Guid id,
        Transaction tx)
    {
        var response = await client.Http.PutAsJsonAsync($"/internal/transactions/{id}", tx);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteTransaction(TransactionsServiceClient client, Guid id)
    {
        var response = await client.Http.DeleteAsync($"/internal/transactions/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        return Results.NoContent();
    }
}

