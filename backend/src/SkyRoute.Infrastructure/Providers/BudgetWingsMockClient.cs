using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Services;

namespace SkyRoute.Infrastructure.Providers;

public sealed class BudgetWingsMockClient(BudgetWingsPricingStrategy pricingStrategy) : IFlightProviderClient
{
    public string ProviderCode => "BUDGET_WINGS";

    public Task<IReadOnlyCollection<ProviderFlightOffer>> SearchAsync(
        FlightProviderSearchContext context,
        CancellationToken cancellationToken)
    {
        var offers = DeterministicFlightFactory.CreateOffers(
            context,
            ProviderCode,
            "BudgetWings",
            "BW",
            -10,
            pricingStrategy);

        return Task.FromResult(offers);
    }
}