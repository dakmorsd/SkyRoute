namespace SkyRoute.Application.DTOs.Flights;

public sealed record FlightSearchResponse(IReadOnlyCollection<FlightOfferDto> Offers);