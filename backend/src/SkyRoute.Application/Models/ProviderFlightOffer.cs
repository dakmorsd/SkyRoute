using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.Models;

public sealed record ProviderFlightOffer(
    string ProviderCode,
    string ProviderName,
    string FlightNumber,
    string OriginAirportCode,
    string OriginCountryCode,
    string DestinationAirportCode,
    string DestinationCountryCode,
    DateTimeOffset DepartureTimeUtc,
    DateTimeOffset ArrivalTimeUtc,
    CabinClass CabinClass,
    RouteType RouteType,
    decimal PerPassengerPrice,
    decimal TotalPrice);