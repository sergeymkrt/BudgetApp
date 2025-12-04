using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Rules.DTOs;
using BudgetApp.Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Rules.Commands.CreateRule;

public record CreateRuleCommand(
    int CategoryId,
    string Pattern,
    int Priority = 0,
    TransactionType? AppliesTo = null
) : IRequest<CategoryRuleDto>;

public class CreateRuleCommandHandler : IRequestHandler<CreateRuleCommand, CategoryRuleDto>
{
    private readonly IApplicationDbContext _context;

    public CreateRuleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryRuleDto> Handle(
        CreateRuleCommand request,
        CancellationToken cancellationToken)
    {
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
            throw new ValidationException($"Category {request.CategoryId} does not exist.");

        var rule = new CategoryRule
        {
            CategoryId = request.CategoryId,
            Pattern = request.Pattern,
            Priority = request.Priority,
            AppliesTo = request.AppliesTo
        };

        _context.CategoryRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);

        var category = await _context.Categories.FindAsync([request.CategoryId], cancellationToken);

        return new CategoryRuleDto(
            rule.Id,
            rule.CategoryId,
            category?.Name,
            rule.Pattern,
            rule.Priority,
            rule.AppliesTo);
    }
}
