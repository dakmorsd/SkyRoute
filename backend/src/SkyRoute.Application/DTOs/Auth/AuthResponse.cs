namespace SkyRoute.Application.DTOs.Auth;

public sealed record AuthResponse(string Token, DateTimeOffset ExpiresAtUtc, UserProfileDto User);