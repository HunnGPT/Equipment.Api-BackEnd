using MediatR;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Commands;

public record UpdateEquipmentCommand(
    int Id,
    string Code,
    string Name,
    string Status,
    int CategoryId
) : IRequest<EquipmentModel?>;