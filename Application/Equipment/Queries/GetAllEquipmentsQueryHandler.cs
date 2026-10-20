using MediatR;
using Equipment.Api.DTOs;
using Equipment.Api.DTOs.Paging;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;


namespace Equipment.Api.Application.Equipment.Queries;

public class GetAllEquipmentsQueryHandler
    : IRequestHandler<GetAllEquipmentsQuery, PagedResult<EquipmentDto>>
{
    private readonly AppDbContext _dbContext;

    public GetAllEquipmentsQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EquipmentDto>> Handle(
        GetAllEquipmentsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Equipments
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();

            query = query.Where(e =>
                e.Name.ToLower().Contains(search) ||
                e.Code.ToLower().Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            query = query.Where(e => e.Status == request.Status);
        }

        var projected = query.Select(e => new EquipmentDto
        {
            Id = e.Id,
            Code = e.Code,
            Name = e.Name,
            Status = e.Status,
            CategoryId = e.CategoryId,
            CategoryName = e.Category.Name
        });

        return await ApplySort(projected, request.SortBy, request.Descending)
            .ToPagedAsync(request, cancellationToken);
    }

    private static IQueryable<EquipmentDto> ApplySort(
    IQueryable<EquipmentDto> query,
    string? sortBy,
    bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "code" => descending
                ? query.OrderByDescending(e => e.Code)
                : query.OrderBy(e => e.Code),

            "status" => descending
                ? query.OrderByDescending(e => e.Status)
                : query.OrderBy(e => e.Status),

            "category" => descending
                ? query.OrderByDescending(e => e.CategoryName)
                : query.OrderBy(e => e.CategoryName),

            _ => descending
                ? query.OrderByDescending(e => e.Name)
                : query.OrderBy(e => e.Name)
        };
    }
}