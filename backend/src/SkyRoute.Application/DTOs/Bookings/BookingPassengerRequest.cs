namespace SkyRoute.Application.DTOs.Bookings;

public sealed record BookingPassengerRequest(string FullName, string Email, string DocumentNumber);