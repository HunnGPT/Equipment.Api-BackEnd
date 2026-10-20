using MediatR;
using Equipment.Api.DTOs;
using Equipment.Api.DTOs.Paging;

namespace Equipment.Api.Application.Category.Queries;

public class GetAllCategoriesQuery : PagedRequest, IRequest<PagedResult<CategoryDto>>
{
    public string? Search { get; set; }
}