using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.DTOs.Flights;

public sealed record FlightOfferDto(
    string OfferToken,
    string ProviderCode,
    string ProviderName,
    string FlightNumber,
    string OriginAirportCode,
    string DestinationAirportCode,
    DateTimeOffset DepartureTimeUtc,
    DateTimeOffset ArrivalTimeUtc,
    int DurationMinutes,
    CabinClass CabinClass,
    RouteType RouteType,
    PriceBreakdownDto Pricing);