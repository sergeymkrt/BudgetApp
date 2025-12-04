using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name, string? ColorHex) : IRequest<bool>;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public UpdateCategoryCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([request.Id], cancellationToken);

        if (category is null)
            throw new NotFoundException("Category", request.Id);

        category.Name = request.Name;
        category.ColorHex = request.ColorHex;

        await _context.SaveChangesAsync(cancellationToken);
        
        _cache.Remove(CacheKeys.AllCategories);
        
        return true;
    }
}
