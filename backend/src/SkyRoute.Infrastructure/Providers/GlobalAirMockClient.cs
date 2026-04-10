using System.Net.Http.Json;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;

namespace SkyRoute.Infrastructure.Providers;

/// <summary>
/// Integrates with the GlobalAir provider API via HTTP.
/// In this demo, the endpoint is a mock controller running inside the same host.
/// The architecture mirrors a real multi-provider aggregation scenario.
/// </summary>
public sealed class GlobalAirProviderClient(HttpClient httpClient) : IFlightProviderClient
{
    public string ProviderCode => "GLOBAL_AIR";

    public async Task<IReadOnlyCollection<ProviderFlightOffer>> SearchAsync(
        FlightProviderSearchContext context,
        CancellationToken cancellationToken)
    {
        var request = new
        {
            OriginCode = context.Origin.Code,
            OriginCountryCode = context.Origin.CountryCode,
            DestinationCode = context.Destination.Code,
            DestinationCountryCode = context.Destination.CountryCode,
            context.DepartureDate,
            context.PassengerCount,
            context.CabinClass,
            context.RouteType,
        };

        var response = await httpClient.PostAsJsonAsync("search", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<ProviderApiSearchResponse>(cancellationToken)
            ?? throw new InvalidOperationException("GlobalAir API returned an empty response.");

        return body.Flights.Select(flight => new ProviderFlightOffer(
            ProviderCode,
            "GlobalAir",
            flight.FlightNumber,
            flight.OriginAirportCode,
            context.Origin.CountryCode,
            flight.DestinationAirportCode,
            context.Destination.CountryCode,
            flight.DepartureTimeUtc,
            flight.ArrivalTimeUtc,
            flight.CabinClass,
            flight.RouteType,
            flight.PerPassengerPrice,
            flight.TotalPrice)).ToArray();
    }
}