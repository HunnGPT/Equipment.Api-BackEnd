using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Queries;

public class GetEquipmentByIdQueryHandler
    : IRequestHandler<GetEquipmentByIdQuery, EquipmentModel?>
{
    private readonly AppDbContext _dbContext;

    public GetEquipmentByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EquipmentModel?> Handle(
        GetEquipmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Equipments
            .AsNoTracking()
            .FirstOrDefaultAsync(
                equipment => equipment.Id == request.Id,
                cancellationToken);
    }
}