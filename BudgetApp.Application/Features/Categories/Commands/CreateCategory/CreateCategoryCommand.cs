using BudgetApp.Application.Common.Interfaces;
using BudgetApp.Application.Features.Categories.DTOs;
using BudgetApp.Domain.Models;
using MediatR;

namespace BudgetApp.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? ColorHex) : IRequest<CategoryDto>;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly IApplicationDbContext _context;

    public CreateCategoryCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = request.Name,
            ColorHex = request.ColorHex
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.ColorHex);
    }
}
