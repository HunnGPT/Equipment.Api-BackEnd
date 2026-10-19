using MediatR;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Commands;

public record CreateEquipmentCommand(
    string Code,
    string Name,
    string Status,
    int CategoryId
) : IRequest<EquipmentModel>;