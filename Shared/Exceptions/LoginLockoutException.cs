namespace Wang.Seamas.Shared.Exceptions;

public class LoginLockoutException : Exception
{
    public int RemainingMinutes { get; }

    public LoginLockoutException(string message, int remainingMinutes, Exception? innerException = null) 
        : base(message, innerException)
    {
        RemainingMinutes = remainingMinutes;
    }
}
