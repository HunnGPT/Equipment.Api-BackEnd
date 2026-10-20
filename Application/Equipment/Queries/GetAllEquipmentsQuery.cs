using MediatR;
using Equipment.Api.DTOs;
using Equipment.Api.DTOs.Paging;

namespace Equipment.Api.Application.Equipment.Queries;

public class GetAllEquipmentsQuery : PagedRequest, IRequest<PagedResult<EquipmentDto>>
{
    public string? Search { get; set; }

    public int? CategoryId { get; set; }

    public string? Status { get; set; }
}