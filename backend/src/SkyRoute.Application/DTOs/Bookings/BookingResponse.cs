using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.DTOs.Bookings;

public sealed record BookingResponse(
    string BookingReferenceCode,
    string ProviderCode,
    string FlightNumber,
    RouteType RouteType,
    int PassengerCount,
    decimal PerPassengerPrice,
    decimal TotalPrice);