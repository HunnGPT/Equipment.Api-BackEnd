namespace Equipment.Api.Application.Category.Exceptions;

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException()
        : base("Category not found.")
    {
    }
}
