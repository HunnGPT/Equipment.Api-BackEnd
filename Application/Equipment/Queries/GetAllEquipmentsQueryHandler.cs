using MediatR;
using Equipment.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;

namespace Equipment.Api.Application.Equipment.Queries;

public class GetAllEquipmentsQueryHandler
    : IRequestHandler<GetAllEquipmentsQuery, List<EquipmentDto>>
{
    private readonly AppDbContext _dbContext;

    public GetAllEquipmentsQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<EquipmentDto>> Handle(
        GetAllEquipmentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Equipments
            .AsNoTracking()
            .Select(e => new EquipmentDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                Status = e.Status,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name
            })
            .ToListAsync(cancellationToken);
    }
}