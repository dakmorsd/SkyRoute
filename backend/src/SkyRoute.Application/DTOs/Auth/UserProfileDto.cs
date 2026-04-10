namespace SkyRoute.Application.DTOs.Auth;

public sealed record UserProfileDto(Guid Id, string FullName, string Email);