using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Budgets.DTOs;
using BudgetApp.Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Budgets.Commands.CreateBudget;

public record CreateBudgetCommand(
    int CategoryId,
    decimal LimitAmount,
    int Year,
    int Month
) : IRequest<BudgetDto>;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IApplicationDbContext _context;

    public CreateBudgetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new ValidationException($"Category {request.CategoryId} does not exist.");

        var budget = new Budget
        {
            CategoryId = request.CategoryId,
            LimitAmount = request.LimitAmount,
            Year = request.Year,
            Month = request.Month
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync(cancellationToken);

        var category = await _context.Categories.FindAsync([request.CategoryId], cancellationToken);

        return new BudgetDto(
            budget.Id,
            budget.CategoryId,
            category?.Name,
            budget.LimitAmount,
            budget.Year,
            budget.Month);
    }
}
