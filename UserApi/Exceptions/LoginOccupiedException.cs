namespace UserApi.Exceptions;

public class LoginOccupiedException: Exception
{
    public LoginOccupiedException()
    {
    }

    public LoginOccupiedException(string message)
        : base(message)
    {
    }

    public LoginOccupiedException(string message, Exception inner)
        : base(message, inner)
    {
    }
}