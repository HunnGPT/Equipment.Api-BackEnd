using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Equipment.Queries;

public record GetEquipmentByIdQuery(int Id) : IRequest<EquipmentDto?>;