using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Rules.DTOs;
using BudgetApp.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Rules.Queries.ClassifyTransaction;

public record ClassifyTransactionQuery(
    string Description,
    string? Merchant,
    decimal Amount,
    TransactionType Type
) : IRequest<ClassificationResultDto?>;

public class ClassifyTransactionQueryHandler
    : IRequestHandler<ClassifyTransactionQuery, ClassificationResultDto?>
{
    private readonly IApplicationDbContext _context;

    public ClassifyTransactionQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClassificationResultDto?> Handle(
        ClassifyTransactionQuery request,
        CancellationToken cancellationToken)
    {
        var text = (request.Description + " " + request.Merchant).ToLowerInvariant();

        var rules = await _context.CategoryRules
            .Where(r => r.AppliesTo == null || r.AppliesTo == request.Type)
            .OrderByDescending(r => r.Priority)
            .ToListAsync(cancellationToken);

        foreach (var rule in rules)
        {
            if (string.IsNullOrWhiteSpace(rule.Pattern))
                continue;

            if (text.Contains(rule.Pattern.ToLowerInvariant()))
            {
                return new ClassificationResultDto(rule.CategoryId, rule.Priority);
            }
        }

        return null;
    }
}
