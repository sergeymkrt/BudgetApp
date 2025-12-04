using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Features.Rules.Commands.CreateRule;
using BudgetApp.Application.Features.Rules.Commands.DeleteRule;
using BudgetApp.Application.Features.Rules.Commands.UpdateRule;
using BudgetApp.Application.Features.Rules.DTOs;
using BudgetApp.Application.Features.Rules.Queries.ClassifyTransaction;
using BudgetApp.Application.Features.Rules.Queries.GetRuleById;
using BudgetApp.Application.Features.Rules.Queries.GetRules;
using BudgetApp.Domain.Models;
using FluentValidation;
using MediatR;

namespace BudgetApp.RulesService.Endpoints;

public static class RuleEndpoints
{
    public static WebApplication MapRuleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/rules")
            .WithTags("Rules");

        group.MapGet("/", GetRules)
            .WithName("GetRules")
            .WithSummary("Get all categorization rules");

        group.MapGet("/{id:int}", GetRuleById)
            .WithName("GetRuleById")
            .WithSummary("Get a rule by ID");

        group.MapPost("/", CreateRule)
            .WithName("CreateRule")
            .WithSummary("Create a new categorization rule");

        group.MapPut("/{id:int}", UpdateRule)
            .WithName("UpdateRule")
            .WithSummary("Update an existing rule");

        group.MapDelete("/{id:int}", DeleteRule)
            .WithName("DeleteRule")
            .WithSummary("Delete a rule");

        group.MapPost("/classify", ClassifyTransaction)
            .WithName("ClassifyTransaction")
            .WithSummary("Classify a transaction using rules");

        return app;
    }

    private static async Task<IResult> GetRules(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetRulesQuery(), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetRuleById(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetRuleByIdQuery(id), ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateRule(
        ISender sender,
        CreateRuleRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new CreateRuleCommand(
                request.CategoryId,
                request.Pattern,
                request.Priority,
                request.AppliesTo);

            var result = await sender.Send(command, ct);
            return Results.Created($"/internal/rules/{result.Id}", result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    private static async Task<IResult> UpdateRule(
        ISender sender,
        int id,
        UpdateRuleRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new UpdateRuleCommand(
                id,
                request.CategoryId,
                request.Pattern,
                request.Priority,
                request.AppliesTo);

            await sender.Send(command, ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteRule(
        ISender sender,
        int id,
        CancellationToken ct = default)
    {
        try
        {
            await sender.Send(new DeleteRuleCommand(id), ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> ClassifyTransaction(
        ISender sender,
        ClassifyTransactionRequest request,
        CancellationToken ct = default)
    {
        var query = new ClassifyTransactionQuery(
            request.Description,
            request.Merchant,
            request.Amount,
            request.Type);

        var result = await sender.Send(query, ct);
        
        // Return a proper JSON response even when no match is found
        return Results.Ok(result ?? new ClassificationResultDto(null, 0));
    }
}

public record CreateRuleRequest(
    int CategoryId,
    string Pattern,
    int Priority = 0,
    TransactionType? AppliesTo = null);

public record UpdateRuleRequest(
    int CategoryId,
    string Pattern,
    int Priority,
    TransactionType? AppliesTo);

public record ClassifyTransactionRequest(
    string Description,
    string? Merchant,
    decimal Amount,
    TransactionType Type);

