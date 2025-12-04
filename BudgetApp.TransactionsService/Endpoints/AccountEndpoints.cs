using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Features.Accounts.Commands.CreateAccount;
using BudgetApp.Application.Features.Accounts.Commands.DeleteAccount;
using BudgetApp.Application.Features.Accounts.Commands.UpdateAccount;
using BudgetApp.Application.Features.Accounts.Queries.GetAccountById;
using BudgetApp.Application.Features.Accounts.Queries.GetAccounts;
using MediatR;

namespace BudgetApp.TransactionsService.Endpoints;

public static class AccountEndpoints
{
    public static WebApplication MapAccountEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/internal/accounts")
            .WithTags("Accounts");

        group.MapGet("/", GetAccounts)
            .WithName("GetAccounts")
            .WithSummary("Get all accounts");

        group.MapGet("/{id:guid}", GetAccountById)
            .WithName("GetAccountById")
            .WithSummary("Get an account by ID");

        group.MapPost("/", CreateAccount)
            .WithName("CreateAccount")
            .WithSummary("Create a new account");

        group.MapPut("/{id:guid}", UpdateAccount)
            .WithName("UpdateAccount")
            .WithSummary("Update an existing account");

        group.MapDelete("/{id:guid}", DeleteAccount)
            .WithName("DeleteAccount")
            .WithSummary("Delete an account");

        return app;
    }

    private static async Task<IResult> GetAccounts(
        ISender sender,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAccountsQuery(), ct);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAccountById(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAccountByIdQuery(id), ct);
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }

    private static async Task<IResult> CreateAccount(
        ISender sender,
        CreateAccountRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateAccountCommand(request.Name, request.Currency);
        var result = await sender.Send(command, ct);
        return Results.Created($"/internal/accounts/{result.Id}", result);
    }

    private static async Task<IResult> UpdateAccount(
        ISender sender,
        Guid id,
        UpdateAccountRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var command = new UpdateAccountCommand(id, request.Name, request.Currency);
            await sender.Send(command, ct);
            return Results.NoContent();
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }

    private static async Task<IResult> DeleteAccount(
        ISender sender,
        Guid id,
        CancellationToken ct = default)
    {
        try
        {
            var result = await sender.Send(new DeleteAccountCommand(id), ct);
            return result.Match<IResult>(
                () => Results.NoContent(),
                errors => Results.BadRequest(errors.FirstOrDefault()));
        }
        catch (NotFoundException)
        {
            return Results.NotFound();
        }
    }
}

public record CreateAccountRequest(string Name, string? Currency);
public record UpdateAccountRequest(string Name, string? Currency);

