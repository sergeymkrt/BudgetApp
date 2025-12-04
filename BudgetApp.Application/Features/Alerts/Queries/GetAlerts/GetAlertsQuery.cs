using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Alerts.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Application.Features.Alerts.Queries.GetAlerts;

public record GetAlertsQuery(int Take = 100) : IRequest<IReadOnlyList<AlertDto>>;

public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, IReadOnlyList<AlertDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAlertsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AlertDto>> Handle(
        GetAlertsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Alerts
            .Include(a => a.Category)
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.Take)
            .Select(a => new AlertDto(
                a.Id,
                a.CreatedAt,
                a.Type,
                a.Message,
                a.CategoryId,
                a.Category != null ? a.Category.Name : null))
            .ToListAsync(cancellationToken);
    }
}
