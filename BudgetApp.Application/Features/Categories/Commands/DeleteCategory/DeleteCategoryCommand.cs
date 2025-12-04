using BudgetApp.Application.Common.Caching;
using BudgetApp.Application.Common.Exceptions;
using BudgetApp.Application.Common.Interfaces;
using MediatR;

namespace BudgetApp.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<bool>;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public DeleteCategoryCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync([request.Id], cancellationToken);

        if (category is null)
            throw new NotFoundException("Category", request.Id);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _cache.Remove(CacheKeys.AllCategories);
        
        return true;
    }
}
