namespace SkyRoute.Application.Models;

public sealed record AuthToken(string Token, DateTimeOffset ExpiresAtUtc);