namespace Equipment.Api.Application.Category.Exceptions;

public class CategoryInUseException : Exception
{
    public CategoryInUseException()
        : base("Category is currently in use.")
    {
    }
}