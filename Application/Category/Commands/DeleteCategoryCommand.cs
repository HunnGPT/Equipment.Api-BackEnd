using MediatR;

namespace Equipment.Api.Application.Category.Commands;

public class DeleteCategoryCommand : IRequest<bool>
{
    public int Id { get; set; }

    public DeleteCategoryCommand(int id)
    {
        Id = id;
    }
}