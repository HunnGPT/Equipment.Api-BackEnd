using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Commands;
using Equipment.Api.Application.Category.Exceptions;

namespace Equipment.Api.Application.Category.Handlers;

public class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteCategoryCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException();
        }

        var hasEquipments = await _dbContext.Equipments
            .AnyAsync(e => e.CategoryId == category.Id, cancellationToken);

        if (hasEquipments)
        {
            throw new CategoryInUseException();
        }

        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}