using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class AccountGatewayEndpoints
{
    public static WebApplication MapAccountGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/accounts")
            .WithTags("Accounts");

        group.MapGet("/", GetAccounts)
            .WithSummary("Get all accounts");

        group.MapGet("/{id:guid}", GetAccountById)
            .WithSummary("Get an account by ID");

        group.MapPost("/", CreateAccount)
            .WithSummary("Create a new account");

        group.MapPut("/{id:guid}", UpdateAccount)
            .WithSummary("Update an account");

        group.MapDelete("/{id:guid}", DeleteAccount)
            .WithSummary("Delete an account");

        return app;
    }

    private static async Task<IResult> GetAccounts(TransactionsServiceClient client)
    {
        var accounts = await client.Http.GetFromJsonAsync<List<Account>>("/internal/accounts");
        return Results.Ok(accounts ?? []);
    }

    private static async Task<IResult> GetAccountById(TransactionsServiceClient client, Guid id)
    {
        var response = await client.Http.GetAsync($"/internal/accounts/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        var account = await response.Content.ReadFromJsonAsync<Account>();
        return Results.Ok(account);
    }

    private static async Task<IResult> CreateAccount(TransactionsServiceClient client, Account account)
    {
        var response = await client.Http.PostAsJsonAsync("/internal/accounts", account);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<Account>();
        return Results.Created($"/accounts/{created?.Id}", created);
    }

    private static async Task<IResult> UpdateAccount(TransactionsServiceClient client, Guid id, Account account)
    {
        var response = await client.Http.PutAsJsonAsync($"/internal/accounts/{id}", account);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteAccount(TransactionsServiceClient client, Guid id)
    {
        var response = await client.Http.DeleteAsync($"/internal/accounts/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            return Results.Problem($"Downstream error: {response.StatusCode} - {content}");
        }

        return Results.NoContent();
    }
}

