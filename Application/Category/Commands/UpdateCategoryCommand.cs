using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Category.Commands;

public class UpdateCategoryCommand : IRequest<CategoryDto?>
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }

    public UpdateCategoryCommand(int id, UpdateCategoryRequest request)
    {
        Id = id;
        Name = request.Name;
        Description = request.Description;
    }
}