using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Category.Exceptions;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Commands;

public class UpdateEquipmentCommandHandler
    : IRequestHandler<UpdateEquipmentCommand, EquipmentModel?>
{
    private readonly AppDbContext _dbContext;

    public UpdateEquipmentCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EquipmentModel?> Handle(
        UpdateEquipmentCommand request,
        CancellationToken cancellationToken)
    {
        var equipment = await _dbContext.Equipments.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (equipment == null)
        {
            return null;
        }

        var categoryExists = await _dbContext.Categories
            .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            throw new CategoryNotFoundException();
        }

        equipment.Code = request.Code;
        equipment.Name = request.Name;
        equipment.Status = request.Status;
        equipment.CategoryId = request.CategoryId;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return equipment;
    }
}