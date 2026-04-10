using SkyRoute.Application.DTOs.Flights;

namespace SkyRoute.Application.Interfaces;

public interface IFlightSearchService
{
    Task<FlightSearchResponse> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken);
}