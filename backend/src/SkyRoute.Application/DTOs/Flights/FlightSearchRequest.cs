using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.DTOs.Flights;

public sealed record FlightSearchRequest(
    string OriginAirportCode,
    string DestinationAirportCode,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass);