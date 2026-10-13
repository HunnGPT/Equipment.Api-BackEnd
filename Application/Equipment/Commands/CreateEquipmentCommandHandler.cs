using MediatR;
using Equipment.Api.Infrastructure.Data;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Commands;

public class CreateEquipmentCommandHandler
    : IRequestHandler<CreateEquipmentCommand, EquipmentModel>
{
    private readonly AppDbContext _dbContext;

    public CreateEquipmentCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EquipmentModel> Handle(
        CreateEquipmentCommand request,
        CancellationToken cancellationToken)
    {
        var equipment = new EquipmentModel
        {
            Code = request.Code,
            Name = request.Name,
            Status = request.Status
        };

        _dbContext.Equipments.Add(equipment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return equipment;
    }
}