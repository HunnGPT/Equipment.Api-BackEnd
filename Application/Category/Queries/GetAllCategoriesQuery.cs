using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Category.Queries;

public class GetAllCategoriesQuery : IRequest<CategoryListResponse>
{
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}