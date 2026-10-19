using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;

namespace Equipment.Api.Application.Equipment.Commands;

public class DeleteEquipmentCommandHandler
    : IRequestHandler<DeleteEquipmentCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public DeleteEquipmentCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        DeleteEquipmentCommand request,
        CancellationToken cancellationToken)
    {
        var equipment = await _dbContext.Equipments.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (equipment == null)
        {
            return false;
        }

        _dbContext.Equipments.Remove(equipment);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}