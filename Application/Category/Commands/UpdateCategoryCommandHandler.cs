using MediatR;
using Equipment.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Commands;
using Equipment.Api.Application.Category.Exceptions;

namespace Equipment.Api.Application.Category.Handlers;

public class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, CategoryDto?>
{
    private readonly AppDbContext _dbContext;

    public UpdateCategoryCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto?> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            throw new CategoryNotFoundException();
        }

        var name = string.Join(
            " ",
            request.Name.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));

        var normalizedName = name.ToLower();

        var isDuplicate = await _dbContext.Categories
            .AnyAsync(
                c => c.Id != request.Id &&
                    c.Name.ToLower() == normalizedName,
                cancellationToken);

        if (isDuplicate)
        {
            throw new CategoryAlreadyExistsException();
        }

        category.Name = name;
        category.Description = request.Description?.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            EquipmentCount = await _dbContext.Equipments
                .CountAsync(e => e.CategoryId == category.Id, cancellationToken)
        };
    }
}