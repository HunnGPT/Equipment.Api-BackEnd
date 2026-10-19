namespace Equipment.Api.Application.Auth.Exceptions;

public class UsernameAlreadyExistsException : Exception
{
    public UsernameAlreadyExistsException(string message)
        : base(message)
    {
    }
}