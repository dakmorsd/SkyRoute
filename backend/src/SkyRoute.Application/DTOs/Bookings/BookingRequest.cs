namespace SkyRoute.Application.DTOs.Bookings;

public sealed record BookingRequest(string OfferToken, IReadOnlyCollection<BookingPassengerRequest> Passengers);