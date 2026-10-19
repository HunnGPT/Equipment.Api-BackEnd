using MediatR;
using Equipment.Api.DTOs;
namespace Equipment.Api.Application.Equipment.Queries;

public record GetAllEquipmentsQuery() : IRequest<List<EquipmentDto>>;