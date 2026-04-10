using SkyRoute.Application.DTOs.Airports;

namespace SkyRoute.Application.Interfaces;

public interface IAirportQueryService
{
    Task<IReadOnlyCollection<AirportDto>> GetAllAsync(CancellationToken cancellationToken);
}