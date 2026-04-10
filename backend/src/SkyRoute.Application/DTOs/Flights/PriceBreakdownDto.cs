namespace SkyRoute.Application.DTOs.Flights;

public sealed record PriceBreakdownDto(decimal PerPassengerPrice, int PassengerCount, decimal TotalPrice, string Currency);