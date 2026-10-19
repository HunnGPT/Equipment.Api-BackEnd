using MediatR;
using EquipmentModel = Equipment.Api.Domain.Entities.Equipment;

namespace Equipment.Api.Application.Equipment.Commands;

public record DeleteEquipmentCommand(int Id) : IRequest<bool>;