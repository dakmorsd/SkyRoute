using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Services;

namespace SkyRoute.Infrastructure.Providers;

public sealed class GlobalAirMockClient(GlobalAirPricingStrategy pricingStrategy) : IFlightProviderClient
{
    public string ProviderCode => "GLOBAL_AIR";

    public Task<IReadOnlyCollection<ProviderFlightOffer>> SearchAsync(
        FlightProviderSearchContext context,
        CancellationToken cancellationToken)
    {
        var offers = DeterministicFlightFactory.CreateOffers(
            context,
            ProviderCode,
            "GlobalAir",
            "GA",
            22,
            pricingStrategy);

        return Task.FromResult(offers);
    }
}