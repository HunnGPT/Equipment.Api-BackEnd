using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Equipment.Queries;

public class GetEquipmentByIdQueryHandler
    : IRequestHandler<GetEquipmentByIdQuery, EquipmentDto?>
{
    private readonly AppDbContext _dbContext;

    public GetEquipmentByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EquipmentDto?> Handle(
        GetEquipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Equipments
            .AsNoTracking()
            .Where(e => e.Id == request.Id)
            .Select(e => new EquipmentDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                Status = e.Status,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}