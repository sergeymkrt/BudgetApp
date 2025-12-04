using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Features.Categories.Commands.CreateCategory;
using BudgetApp.Application.Features.Categories.Commands.DeleteCategory;
using BudgetApp.Application.Features.Categories.Commands.UpdateCategory;
using BudgetApp.Application.Features.Categories.Queries.GetCategories;
using BudgetApp.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;

namespace BudgetApp.TransactionsService.Endpoints;

public static class CategoryEndpoints
{
    public static WebApplication MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/categories")
            .WithTags("Categories");

        group.MapGet("/", GetCategories)
            .WithName("GetCategories")
            .WithSummary("Get all categories");

        group.MapGet("/{id:int}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithSummary("Get a category by ID");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        group.MapPut("/{id:int}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithSummary("Update an existing category");

        group.MapDelete("/{id:int}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithSummary("Delete a category");

        return app;
    }

    private static async Task<IResult> GetCategories(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCategoriesQuery(), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCategoryById(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateCategory(
        ISender sender,
        CreateCategoryRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateCategoryCommand(request.Name, request.ColorHex);
        var result = await sender.Send(command, ct);
        return Results.Created($"/internal/categories/{result.Id}", result);
    }

    private static async Task<IResult> UpdateCategory(
        ISender sender,
        int id,
        UpdateCategoryRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new UpdateCategoryCommand(id, request.Name, request.ColorHex);
            await sender.Send(command, ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteCategory(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        try
        {
            await sender.Send(new DeleteCategoryCommand(id), ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }
}

public record CreateCategoryRequest(string Name, string? ColorHex);
public record UpdateCategoryRequest(string Name, string? ColorHex);

