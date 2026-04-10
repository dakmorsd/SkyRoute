namespace SkyRoute.Application.Models;

public sealed record AuthenticatedUser(Guid Id, string FullName, string Email);