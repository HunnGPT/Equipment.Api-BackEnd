using MediatR;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Queries;

public record GetEquipmentByIdQuery(int Id) : IRequest<EquipmentModel?>;