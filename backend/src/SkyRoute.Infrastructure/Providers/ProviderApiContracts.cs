using SkyRoute.Domain.Enums;

namespace SkyRoute.Infrastructure.Providers;

/// <summary>
/// Maps the JSON response returned by an external provider API.
/// Each provider mock endpoint returns this shape.
/// </summary>
internal sealed record ProviderApiSearchResponse(IReadOnlyCollection<ProviderApiFlightResult> Flights);

internal sealed record ProviderApiFlightResult(
    string FlightNumber,
    string OriginAirportCode,
    string DestinationAirportCode,
    DateTimeOffset DepartureTimeUtc,
    DateTimeOffset ArrivalTimeUtc,
    CabinClass CabinClass,
    RouteType RouteType,
    decimal PerPassengerPrice,
    decimal TotalPrice);
