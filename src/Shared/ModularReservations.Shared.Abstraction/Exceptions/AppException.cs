namespace ModularReservations.Shared.Exceptions;

public abstract class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}
