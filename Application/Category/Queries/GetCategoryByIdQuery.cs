using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Category.Queries;

public class GetCategoryByIdQuery : IRequest<CategoryDto?>
{
    public int Id { get; set; }

    public GetCategoryByIdQuery(int id)
    {
        Id = id;
    }
}