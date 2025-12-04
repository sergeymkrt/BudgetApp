using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Features.Budgets.Commands.CreateBudget;
using BudgetApp.Application.Features.Budgets.Commands.DeleteBudget;
using BudgetApp.Application.Features.Budgets.Commands.UpdateBudget;
using BudgetApp.Application.Features.Budgets.Queries.GetBudgetById;
using BudgetApp.Application.Features.Budgets.Queries.GetBudgets;
using FluentValidation;
using MediatR;

namespace BudgetApp.TransactionsService.Endpoints;

public static class BudgetEndpoints
{
    public static WebApplication MapBudgetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/budgets")
            .WithTags("Budgets");

        group.MapGet("/", GetBudgets)
            .WithName("GetBudgets")
            .WithSummary("Get all budgets, optionally filtered by year/month");

        group.MapGet("/{id:int}", GetBudgetById)
            .WithName("GetBudgetById")
            .WithSummary("Get a budget by ID");

        group.MapPost("/", CreateBudget)
            .WithName("CreateBudget")
            .WithSummary("Create a new budget");

        group.MapPut("/{id:int}", UpdateBudget)
            .WithName("UpdateBudget")
            .WithSummary("Update an existing budget");

        group.MapDelete("/{id:int}", DeleteBudget)
            .WithName("DeleteBudget")
            .WithSummary("Delete a budget");

        return app;
    }

    private static async Task<IResult> GetBudgets(
        ISender sender,
        int? year,
        int? month,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetBudgetsQuery(year, month), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBudgetById(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetBudgetByIdQuery(id), ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateBudget(
        ISender sender,
        CreateBudgetRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new CreateBudgetCommand(
                request.CategoryId,
                request.LimitAmount,
                request.Year,
                request.Month);

            var result = await sender.Send(command, ct);
            return Results.Created($"/internal/budgets/{result.Id}", result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> UpdateBudget(
        ISender sender,
        int id,
        UpdateBudgetRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new UpdateBudgetCommand(
                id,
                request.CategoryId,
                request.LimitAmount,
                request.Year,
                request.Month);

            await sender.Send(command, ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteBudget(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        try
        {
            await sender.Send(new DeleteBudgetCommand(id), ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }
}

public record CreateBudgetRequest(int CategoryId, decimal LimitAmount, int Year, int Month);
public record UpdateBudgetRequest(int CategoryId, decimal LimitAmount, int Year, int Month);

