using SkyRoute.Application.DTOs.Airports;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Application.Services;

public sealed class AirportQueryService(IAirportRepository airportRepository) : IAirportQueryService
{
    public async Task<IReadOnlyCollection<AirportDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var airports = await airportRepository.GetAllAsync(cancellationToken);

        return airports
            .OrderBy(airport => airport.CountryName)
            .ThenBy(airport => airport.City)
            .Select(airport => new AirportDto(
                airport.Code,
                airport.Name,
                airport.City,
                airport.CountryCode,
                airport.CountryName))
            .ToArray();
    }
}