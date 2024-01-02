namespace Endpoint.NET.Exceptions;

public class EndpointException : Exception
{
    public EndpointException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}