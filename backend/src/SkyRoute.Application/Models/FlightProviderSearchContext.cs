using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.Models;

public sealed record FlightProviderSearchContext(
    Airport Origin,
    Airport Destination,
    DateOnly DepartureDate,
    int PassengerCount,
    CabinClass CabinClass,
    RouteType RouteType);