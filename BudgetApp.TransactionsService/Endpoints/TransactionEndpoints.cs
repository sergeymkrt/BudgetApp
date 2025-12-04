using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Features.Transactions.Commands.ApplyCategory;
using BudgetApp.Application.Features.Transactions.Commands.CreateTransaction;
using BudgetApp.Application.Features.Transactions.Commands.DeleteTransaction;
using BudgetApp.Application.Features.Transactions.Commands.UpdateTransaction;
using BudgetApp.Application.Features.Transactions.Queries.GetTransactionById;
using BudgetApp.Application.Features.Transactions.Queries.GetTransactions;
using BudgetApp.Application.Features.Transactions.Queries.GetUncategorizedTransactions;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.TransactionsService.Endpoints;

public static class TransactionEndpoints
{
    public static WebApplication MapTransactionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/transactions")
            .WithTags("Transactions");

        group.MapGet("/", GetTransactions)
            .WithName("GetTransactions")
            .WithSummary("Get recent transactions");

        group.MapGet("/{id:guid}", GetTransactionById)
            .WithName("GetTransactionById")
            .WithSummary("Get a transaction by ID");

        group.MapPost("/", CreateTransaction)
            .WithName("CreateTransaction")
            .WithSummary("Create a new transaction");

        group.MapPut("/{id:guid}", UpdateTransaction)
            .WithName("UpdateTransaction")
            .WithSummary("Update a transaction");

        group.MapDelete("/{id:guid}", DeleteTransaction)
            .WithName("DeleteTransaction")
            .WithSummary("Delete a transaction");

        group.MapGet("/uncategorized", GetUncategorizedTransactions)
            .WithName("GetUncategorizedTransactions")
            .WithSummary("Get transactions pending categorization");

        group.MapPost("/{id:guid}/apply-category", ApplyCategory)
            .WithName("ApplyCategoryToTransaction")
            .WithSummary("Apply a category to a transaction");

        return app;
    }

    private static async Task<IResult> GetTransactions(
        ISender sender,
        int take = 50,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetTransactionsQuery(take), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTransactionById(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetTransactionByIdQuery(id), ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateTransaction(
        ISender sender,
        CreateTransactionRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateTransactionCommand(
            request.AccountId,
            request.Amount,
            request.Type,
            request.CategoryId,
            request.Date,
            request.Description,
            request.Merchant);

        var result = await sender.Send(command, ct);
        return Results.Created($"/internal/transactions/{result.Id}", result);
    }

    private static async Task<IResult> UpdateTransaction(
        ISender sender,
        Guid id,
        UpdateTransactionRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new UpdateTransactionCommand(
                id,
                request.AccountId,
                request.Amount,
                request.Type,
                request.CategoryId,
                request.Date,
                request.Description,
                request.Merchant);

            await sender.Send(command, ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteTransaction(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        try
        {
            await sender.Send(new DeleteTransactionCommand(id), ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> GetUncategorizedTransactions(
        ISender sender,
        int take = 50,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetUncategorizedTransactionsQuery(take), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> ApplyCategory(
        ISender sender,
        Guid id,
        ApplyCategoryRequest request,
        CancellationToken ct = default)
    {
        var command = new ApplyCategoryCommand(id, request.CategoryId, request.Status);
        await sender.Send(command, ct);
        return Results.NoContent();
    }
}

// Request DTOs for binding
public record CreateTransactionRequest(
    Guid AccountId,
    decimal Amount,
    TransactionType Type,
    int? CategoryId,
    DateTimeOffset? Date,
    string Description,
    string? Merchant);

public record UpdateTransactionRequest(
    Guid AccountId,
    decimal Amount,
    TransactionType Type,
    int? CategoryId,
    DateTimeOffset? Date,
    string Description,
    string? Merchant);

public record ApplyCategoryRequest(int CategoryId, string? Status);

