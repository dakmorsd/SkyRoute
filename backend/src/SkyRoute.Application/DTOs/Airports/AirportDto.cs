namespace SkyRoute.Application.DTOs.Airports;

public sealed record AirportDto(
    string Code,
    string Name,
    string City,
    string CountryCode,
    string CountryName);