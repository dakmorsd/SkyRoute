using SkyRoute.Application.Models;

namespace SkyRoute.Application.Interfaces;

public interface IFlightProviderClient
{
    string ProviderCode { get; }

    Task<IReadOnlyCollection<ProviderFlightOffer>> SearchAsync(
        FlightProviderSearchContext context,
        CancellationToken cancellationToken);
}