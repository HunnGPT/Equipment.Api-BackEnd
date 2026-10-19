using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Category.Commands;

public class CreateCategoryCommand : IRequest<CategoryDto>
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public CreateCategoryCommand(CreateCategoryRequest request)
    {
        Name = request.Name;
        Description = request.Description;
    }
}