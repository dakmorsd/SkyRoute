using FluentAssertions;
using SkyRoute.Application.DTOs.Flights;
using SkyRoute.Application.Exceptions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Application.Services;
using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.Tests;

public sealed class FlightSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_preserves_provider_order_for_frontend_sorting()
    {
        var airportRepository = new FakeAirportRepository([
            new Airport { Code = "EZE", City = "Buenos Aires", CountryCode = "AR", CountryName = "Argentina", Name = "Ezeiza" },
            new Airport { Code = "SCL", City = "Santiago", CountryCode = "CL", CountryName = "Chile", Name = "Santiago" }
        ]);

        var offers = new IFlightProviderClient[]
        {
            new FakeFlightProviderClient("GLOBAL_AIR", [
                new ProviderFlightOffer(
                    "GLOBAL_AIR",
                    "GlobalAir",
                    "GA101",
                    "EZE",
                    "AR",
                    "SCL",
                    "CL",
                    DateTimeOffset.UtcNow.AddDays(4),
                    DateTimeOffset.UtcNow.AddDays(4).AddHours(3),
                    CabinClass.Economy,
                    RouteType.International,
                    410m,
                    410m)
            ]),
            new FakeFlightProviderClient("BUDGET_WINGS", [
                new ProviderFlightOffer(
                    "BUDGET_WINGS",
                    "BudgetWings",
                    "BW202",
                    "EZE",
                    "AR",
                    "SCL",
                    "CL",
                    DateTimeOffset.UtcNow.AddDays(4).AddHours(1),
                    DateTimeOffset.UtcNow.AddDays(4).AddHours(4),
                    CabinClass.Economy,
                    RouteType.International,
                    305m,
                    305m)
            ])
        };

        var service = new FlightSearchService(airportRepository, offers, new FakeOfferTokenService());

        var response = await service.SearchAsync(
            new FlightSearchRequest("EZE", "SCL", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(4)), 1, CabinClass.Economy),
            CancellationToken.None);

        response.Offers.Select(offer => offer.FlightNumber).Should().ContainInOrder("GA101", "BW202");
    }

    [Fact]
    public async Task SearchAsync_rejects_past_departure_dates()
    {
        var service = new FlightSearchService(
            new FakeAirportRepository([]),
            Array.Empty<IFlightProviderClient>(),
            new FakeOfferTokenService());

        var action = async () => await service.SearchAsync(
            new FlightSearchRequest("EZE", "SCL", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-1)), 1, CabinClass.Economy),
            CancellationToken.None);

        var exception = await Assert.ThrowsAsync<RequestValidationException>(action);
        exception.Errors.Should().ContainKey(nameof(FlightSearchRequest.DepartureDate));
    }

    private sealed class FakeAirportRepository(IReadOnlyCollection<Airport> airports) : IAirportRepository
    {
        public Task<IReadOnlyCollection<Airport>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult(airports);

        public Task<Airport?> GetByCodeAsync(string code, CancellationToken cancellationToken) =>
            Task.FromResult(airports.SingleOrDefault(airport => airport.Code == code));

        public Task<IReadOnlyCollection<Airport>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken)
        {
            var codeSet = codes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            return Task.FromResult<IReadOnlyCollection<Airport>>(airports.Where(airport => codeSet.Contains(airport.Code)).ToArray());
        }
    }

    private sealed class FakeFlightProviderClient(string providerCode, IReadOnlyCollection<ProviderFlightOffer> offers) : IFlightProviderClient
    {
        public string ProviderCode { get; } = providerCode;

        public Task<IReadOnlyCollection<ProviderFlightOffer>> SearchAsync(FlightProviderSearchContext context, CancellationToken cancellationToken) =>
            Task.FromResult(offers);
    }

    private sealed class FakeOfferTokenService : IOfferTokenService
    {
        public string CreateToken(OfferSnapshot snapshot) => snapshot.FlightNumber;

        public bool TryReadToken(string token, out OfferSnapshot? snapshot)
        {
            snapshot = null;
            return false;
        }
    }
}