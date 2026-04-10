namespace SkyRoute.Application.Exceptions;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(Dictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}