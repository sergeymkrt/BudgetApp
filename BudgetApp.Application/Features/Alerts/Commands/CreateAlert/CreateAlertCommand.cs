using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Alerts.DTOs;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Alerts.Commands.CreateAlert;

public record CreateAlertCommand(
    string Type,
    string Message,
    int? CategoryId
) : IRequest<AlertDto>;

public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, AlertDto>
{
    private readonly IApplicationDbContext _context;

    public CreateAlertCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AlertDto> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var alert = new Alert
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Type = request.Type,
            Message = request.Message,
            CategoryId = request.CategoryId
        };

        _context.Alerts.Add(alert);
        await _context.SaveChangesAsync(cancellationToken);

        return new AlertDto(
            alert.Id,
            alert.CreatedAt,
            alert.Type,
            alert.Message,
            alert.CategoryId,
            null);
    }
}
