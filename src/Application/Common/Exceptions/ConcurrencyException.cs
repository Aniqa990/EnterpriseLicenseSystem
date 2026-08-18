namespace EnterpriseLicenseSystem.Application.Common.Exceptions;

public class ConcurrencyException : Exception
{
    public ConcurrencyException()
        : base("A concurrency conflict occurred.")
    {
    }

    public ConcurrencyException(string message)
        : base(message)
    {
    }

    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
