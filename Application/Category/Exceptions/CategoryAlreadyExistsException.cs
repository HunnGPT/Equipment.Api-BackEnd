namespace Equipment.Api.Application.Category.Exceptions;

public class CategoryAlreadyExistsException : Exception
{
    public CategoryAlreadyExistsException()
        : base("Category name already exists.")
    {
    }
}