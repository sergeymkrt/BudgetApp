using BudgetApp.ApiService.Services;
using BudgetApp.Domain.Models;

namespace BudgetApp.ApiService.Endpoints;

public static class CategoryGatewayEndpoints
{
    public static WebApplication MapCategoryGatewayEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/categories")
            .WithTags("Categories");

        group.MapGet("/", GetCategories)
            .WithSummary("Get all categories");

        group.MapGet("/{id:int}", GetCategoryById)
            .WithSummary("Get a category by ID");

        group.MapPost("/", CreateCategory)
            .WithSummary("Create a new category");

        group.MapPut("/{id:int}", UpdateCategory)
            .WithSummary("Update a category");

        group.MapDelete("/{id:int}", DeleteCategory)
            .WithSummary("Delete a category");

        return app;
    }

    private static async Task<IResult> GetCategories(TransactionsServiceClient client)
    {
        var categories = await client.Http.GetFromJsonAsync<List<Category>>("/internal/categories");
        return Results.Ok(categories ?? []);
    }

    private static async Task<IResult> GetCategoryById(TransactionsServiceClient client, int id)
    {
        var response = await client.Http.GetAsync($"/internal/categories/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        var category = await response.Content.ReadFromJsonAsync<Category>();
        return Results.Ok(category);
    }

    private static async Task<IResult> CreateCategory(TransactionsServiceClient client, Category category)
    {
        var response = await client.Http.PostAsJsonAsync("/internal/categories", category);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<Category>();
        return Results.Created($"/categories/{created?.Id}", created);
    }

    private static async Task<IResult> UpdateCategory(TransactionsServiceClient client, int id, Category category)
    {
        var response = await client.Http.PutAsJsonAsync($"/internal/categories/{id}", category);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return Results.Problem($"Downstream error: {response.StatusCode} - {error}");
        }

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteCategory(TransactionsServiceClient client, int id)
    {
        var response = await client.Http.DeleteAsync($"/internal/categories/{id}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();

        response.EnsureSuccessStatusCode();
        return Results.NoContent();
    }
}

