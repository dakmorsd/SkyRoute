using SkyRoute.Domain.Enums;

namespace SkyRoute.Api.Controllers.MockProviders;

/// <summary>
/// Simulates the response contract that an external airline provider API would return.
/// Each provider returns its own list of flight results.
/// </summary>
public sealed record ProviderSearchResponse(IReadOnlyCollection<ProviderFlightResult> Flights);

public sealed record ProviderFlightResult(
    string FlightNumber,
    string OriginAirportCode,
    string DestinationAirportCode,
    DateTimeOffset DepartureTimeUtc,
    DateTimeOffset ArrivalTimeUtc,
    CabinClass CabinClass,
    RouteType RouteType,
    decimal PerPassengerPrice,
    decimal TotalPrice);
