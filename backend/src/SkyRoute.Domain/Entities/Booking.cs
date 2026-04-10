using SkyRoute.Domain.Enums;

namespace SkyRoute.Domain.Entities;

public sealed class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ReferenceCode { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public AppUser? User { get; set; }
    public string ProviderCode { get; set; } = string.Empty;
    public string ProviderDisplayName { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string OriginAirportCode { get; set; } = string.Empty;
    public string OriginCountryCode { get; set; } = string.Empty;
    public string DestinationAirportCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public DateTimeOffset DepartureTimeUtc { get; set; }
    public DateTimeOffset ArrivalTimeUtc { get; set; }
    public CabinClass CabinClass { get; set; }
    public RouteType RouteType { get; set; }
    public int PassengerCount { get; set; }
    public decimal PerPassengerPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<BookingPassenger> Passengers { get; set; } = new List<BookingPassenger>();
}