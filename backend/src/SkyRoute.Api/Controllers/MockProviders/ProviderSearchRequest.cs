using SkyRoute.Domain.Enums;

namespace SkyRoute.Api.Controllers.MockProviders;

/// <summary>
/// Simulates the request contract that an external airline provider API would accept.
/// </summary>
public sealed record ProviderSearchRequest(
    string OriginCode,
    string OriginCountryCode,
    string DestinationCode,
    string DestinationCountryCode,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass,
    RouteType RouteType);
