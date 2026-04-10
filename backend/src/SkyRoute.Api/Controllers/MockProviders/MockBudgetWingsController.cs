using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Services;
using SkyRoute.Infrastructure.Providers;

namespace SkyRoute.Api.Controllers.MockProviders;

/// <summary>
/// Simulates the external BudgetWings provider API.
/// In production, this would be a separate service owned by BudgetWings.
/// SkyRoute aggregates results from this endpoint alongside other providers.
/// </summary>
[ApiController]
[Route("mock/budgetwings")]
[Tags("Mock Provider — BudgetWings")]
public sealed class MockBudgetWingsController(BudgetWingsPricingStrategy pricingStrategy) : ControllerBase
{
    private const string ProviderCode = "BUDGET_WINGS";
    private const string ProviderName = "BudgetWings";
    private const string FlightPrefix = "BW";
    private const int ProviderBias = -10;

    [HttpPost("search")]
    [ProducesResponseType(typeof(ProviderSearchResponse), StatusCodes.Status200OK)]
    public ActionResult<ProviderSearchResponse> Search(ProviderSearchRequest request)
    {
        var context = BuildContext(request);
        var offers = DeterministicFlightFactory.CreateOffers(context, ProviderCode, ProviderName, FlightPrefix, ProviderBias, pricingStrategy);

        return Ok(MapToResponse(offers));
    }

    private static FlightProviderSearchContext BuildContext(ProviderSearchRequest request)
    {
        var origin = new Airport
        {
            Code = request.OriginCode,
            CountryCode = request.OriginCountryCode,
            Name = request.OriginCode,
            City = request.OriginCode,
            CountryName = request.OriginCountryCode,
        };

        var destination = new Airport
        {
            Code = request.DestinationCode,
            CountryCode = request.DestinationCountryCode,
            Name = request.DestinationCode,
            City = request.DestinationCode,
            CountryName = request.DestinationCountryCode,
        };

        return new FlightProviderSearchContext(origin, destination, request.DepartureDate, request.PassengerCount, request.CabinClass, request.RouteType);
    }

    private static ProviderSearchResponse MapToResponse(IReadOnlyCollection<ProviderFlightOffer> offers)
    {
        var flights = offers.Select(o => new ProviderFlightResult(
            o.FlightNumber,
            o.OriginAirportCode,
            o.DestinationAirportCode,
            o.DepartureTimeUtc,
            o.ArrivalTimeUtc,
            o.CabinClass,
            o.RouteType,
            o.PerPassengerPrice,
            o.TotalPrice)).ToArray();

        return new ProviderSearchResponse(flights);
    }
}
