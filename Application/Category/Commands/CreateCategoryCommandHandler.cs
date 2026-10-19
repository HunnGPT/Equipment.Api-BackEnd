using MediatR;
using Equipment.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Commands;
using Equipment.Api.Application.Category.Exceptions;

using CategoryModel = Equipment.Api.Domain.Entities.Category;

namespace Equipment.Api.Application.Category.Handlers;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly AppDbContext _dbContext;

    public CreateCategoryCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var name = string.Join(
            " ",
            request.Name.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));

        var normalizedName = name.ToLower();

        var isDuplicate = await _dbContext.Categories
            .AnyAsync(
                c => c.Name.ToLower() == normalizedName,
                cancellationToken);

        if (isDuplicate)
        {
            throw new CategoryAlreadyExistsException();
        }

        var category = new CategoryModel
        {
            Name = name,
            Description = request.Description?.Trim()
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            EquipmentCount = 0
        };
    }
}