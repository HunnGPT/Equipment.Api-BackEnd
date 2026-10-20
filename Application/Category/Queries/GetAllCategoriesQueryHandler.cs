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

        var totalCount = await query.CountAsync(cancellationToken);

        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                EquipmentCount = c.Equipments.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}