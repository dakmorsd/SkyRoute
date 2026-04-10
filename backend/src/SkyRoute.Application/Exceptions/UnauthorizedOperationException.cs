namespace SkyRoute.Application.Exceptions;

public sealed class UnauthorizedOperationException(string message) : Exception(message);