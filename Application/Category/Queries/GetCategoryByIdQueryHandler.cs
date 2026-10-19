using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.DTOs;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Queries;

namespace Equipment.Api.Application.Category.Handlers;

public class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly AppDbContext _dbContext;

    public GetCategoryByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CategoryDto?> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                EquipmentCount = c.Equipments.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}