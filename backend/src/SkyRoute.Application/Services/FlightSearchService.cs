using SkyRoute.Application.DTOs.Flights;
using SkyRoute.Application.Exceptions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Services;

namespace SkyRoute.Application.Services;

public sealed class FlightSearchService(
    IAirportRepository airportRepository,
    IEnumerable<IFlightProviderClient> flightProviderClients,
    IOfferTokenService offerTokenService) : IFlightSearchService
{
    public async Task<FlightSearchResponse> SearchAsync(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        ValidateRequest(request);

        var airports = await airportRepository.GetByCodesAsync(
            [request.OriginAirportCode.Trim().ToUpperInvariant(), request.DestinationAirportCode.Trim().ToUpperInvariant()],
            cancellationToken);

        var origin = airports.SingleOrDefault(airport => airport.Code.Equals(request.OriginAirportCode, StringComparison.OrdinalIgnoreCase));
        var destination = airports.SingleOrDefault(airport => airport.Code.Equals(request.DestinationAirportCode, StringComparison.OrdinalIgnoreCase));

        if (origin is null || destination is null)
        {
            throw new NotFoundException("The selected route could not be resolved.");
        }

        var routeType = RouteClassifier.Classify(origin, destination);
        var providerContext = new FlightProviderSearchContext(
            origin,
            destination,
            request.DepartureDate,
            request.PassengerCount,
            request.CabinClass,
            routeType);

        var providerTasks = flightProviderClients
            .Select(client => client.SearchAsync(providerContext, cancellationToken));

        var providerResults = await Task.WhenAll(providerTasks);

        var offers = providerResults
            .SelectMany(result => result)
            .Select(result => MapOffer(result, request.PassengerCount))
            .ToArray();

        return new FlightSearchResponse(offers);
    }

    private FlightOfferDto MapOffer(ProviderFlightOffer offer, int passengerCount)
    {
        var snapshot = new OfferSnapshot(
            offer.ProviderCode,
            offer.ProviderName,
            offer.FlightNumber,
            offer.OriginAirportCode,
            offer.OriginCountryCode,
            offer.DestinationAirportCode,
            offer.DestinationCountryCode,
            offer.DepartureTimeUtc,
            offer.ArrivalTimeUtc,
            offer.CabinClass,
            offer.RouteType,
            passengerCount,
            offer.PerPassengerPrice,
            offer.TotalPrice,
            DateTimeOffset.UtcNow);

        return new FlightOfferDto(
            offerTokenService.CreateToken(snapshot),
            offer.ProviderCode,
            offer.ProviderName,
            offer.FlightNumber,
            offer.OriginAirportCode,
            offer.DestinationAirportCode,
            offer.DepartureTimeUtc,
            offer.ArrivalTimeUtc,
            (int)(offer.ArrivalTimeUtc - offer.DepartureTimeUtc).TotalMinutes,
            offer.CabinClass,
            offer.RouteType,
            new PriceBreakdownDto(offer.PerPassengerPrice, passengerCount, offer.TotalPrice, "USD"));
    }

    private static void ValidateRequest(FlightSearchRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.OriginAirportCode))
        {
            errors[nameof(request.OriginAirportCode)] = ["Origin airport is required."];
        }

        if (string.IsNullOrWhiteSpace(request.DestinationAirportCode))
        {
            errors[nameof(request.DestinationAirportCode)] = ["Destination airport is required."];
        }

        if (!string.IsNullOrWhiteSpace(request.OriginAirportCode) &&
            request.OriginAirportCode.Equals(request.DestinationAirportCode, StringComparison.OrdinalIgnoreCase))
        {
            errors[nameof(request.DestinationAirportCode)] = ["Origin and destination must be different airports."];
        }

        if (request.PassengerCount is < 1 or > 9)
        {
            errors[nameof(request.PassengerCount)] = ["Passenger count must be between 1 and 9."];
        }

        if (request.DepartureDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
        {
            errors[nameof(request.DepartureDate)] = ["Departure date cannot be in the past."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }
}