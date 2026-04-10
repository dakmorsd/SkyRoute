using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using SkyRoute.Application.DTOs.Auth;
using SkyRoute.Application.DTOs.Bookings;
using SkyRoute.Application.DTOs.Flights;
using SkyRoute.Domain.Enums;

namespace SkyRoute.Api.IntegrationTests;

public sealed class FlightApiTests(SkyRouteApiFactory factory) : IClassFixture<SkyRouteApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Fact]
    public async Task Get_airports_returns_seeded_catalog()
    {
        var response = await _client.GetAsync("/api/airports");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var airports = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<AirportContract>>();
        airports.Should().NotBeNull();
        airports!.Should().Contain(airport => airport.Code == "EZE");
        airports.Count.Should().BeGreaterThanOrEqualTo(6);
    }

    [Fact]
    public async Task Booking_endpoint_requires_authentication()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/bookings",
            new BookingRequest("offer-token", [new BookingPassengerRequest("John Doe", "john@example.com", "12345678")]));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Authenticated_user_can_search_and_book_a_flight()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest("demo@skyroute.local", "Travel123!"));
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var authPayload = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);
        authPayload.Should().NotBeNull();

        var searchResponse = await _client.PostAsJsonAsync(
            "/api/flights/search",
            new FlightSearchRequest("EZE", "AEP", DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(3)), 1, CabinClass.Economy));

        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var offersPayload = await searchResponse.Content.ReadFromJsonAsync<FlightSearchResponse>(JsonOptions);
        offersPayload.Should().NotBeNull();
        offersPayload!.Offers.Should().NotBeEmpty();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authPayload!.Token);

        var bookingResponse = await _client.PostAsJsonAsync(
            "/api/bookings",
            new BookingRequest(
                offersPayload.Offers.First().OfferToken,
                [new BookingPassengerRequest("Demo Traveler", "demo@example.com", "12345678")]));

        bookingResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var confirmation = await bookingResponse.Content.ReadFromJsonAsync<BookingResponse>(JsonOptions);
        confirmation.Should().NotBeNull();
        confirmation!.BookingReferenceCode.Should().StartWith("SKY-");
        confirmation.RouteType.Should().Be(RouteType.Domestic);
    }

    public sealed record AirportContract(string Code, string Name, string City, string CountryCode, string CountryName);
}