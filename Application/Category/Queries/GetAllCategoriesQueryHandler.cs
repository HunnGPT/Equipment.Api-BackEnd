using MediatR;
using Equipment.Api.DTOs;
using Equipment.Api.DTOs.Paging;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Queries;

namespace Equipment.Api.Application.Category.Handlers;

public class GetAllCategoriesQueryHandler
    : IRequestHandler<GetAllCategoriesQuery, PagedResult<CategoryDto>>
{
    private readonly AppDbContext _dbContext;

    public GetAllCategoriesQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CategoryDto>> Handle(
        GetAllCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Categories
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(c => c.Name.ToLower().Contains(search.ToLower()));
        }

        var categoryQuery = query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                EquipmentCount = c.Equipments.Count()
            });

        return await categoryQuery.ToPagedAsync(
            request,
            cancellationToken);
    }
}