using FluentAssertions;
using SkyRoute.Application.DTOs.Bookings;
using SkyRoute.Application.Exceptions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Application.Services;
using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Enums;

namespace SkyRoute.Application.Tests;

public sealed class BookingServiceTests
{
    [Fact]
    public async Task BookAsync_rejects_invalid_passport_numbers_for_international_routes()
    {
        var repository = new FakeBookingRepository();
        var offerTokenService = new FakeOfferTokenService(new OfferSnapshot(
            "GLOBAL_AIR",
            "GlobalAir",
            "GA501",
            "EZE",
            "AR",
            "SCL",
            "CL",
            DateTimeOffset.UtcNow.AddDays(5),
            DateTimeOffset.UtcNow.AddDays(5).AddHours(3),
            CabinClass.Economy,
            RouteType.International,
            1,
            280m,
            280m,
            DateTimeOffset.UtcNow));

        var service = new BookingService(repository, offerTokenService);

        var action = async () => await service.BookAsync(
            new BookingRequest("offer-token", [new BookingPassengerRequest("Jane Traveler", "jane@example.com", "12345")]),
            new AuthenticatedUser(Guid.NewGuid(), "Jane Traveler", "jane@example.com"),
            CancellationToken.None);

        var exception = await Assert.ThrowsAsync<RequestValidationException>(action);
        exception.Errors.Should().ContainKey("Passengers[0].DocumentNumber");
    }

    [Fact]
    public async Task BookAsync_persists_booking_and_returns_confirmation()
    {
        var repository = new FakeBookingRepository();
        var offerTokenService = new FakeOfferTokenService(new OfferSnapshot(
            "BUDGET_WINGS",
            "BudgetWings",
            "BW801",
            "EZE",
            "AR",
            "AEP",
            "AR",
            DateTimeOffset.UtcNow.AddDays(2),
            DateTimeOffset.UtcNow.AddDays(2).AddHours(1),
            CabinClass.Business,
            RouteType.Domestic,
            1,
            89.99m,
            89.99m,
            DateTimeOffset.UtcNow));

        var service = new BookingService(repository, offerTokenService);

        var response = await service.BookAsync(
            new BookingRequest("offer-token", [new BookingPassengerRequest("John Flyer", "john@example.com", "12345678")]),
            new AuthenticatedUser(Guid.NewGuid(), "John Flyer", "john@example.com"),
            CancellationToken.None);

        response.ProviderCode.Should().Be("BUDGET_WINGS");
        response.TotalPrice.Should().Be(89.99m);
        repository.StoredBooking.Should().NotBeNull();
        repository.StoredBooking!.Passengers.Should().ContainSingle();
        repository.SaveChangesCalled.Should().BeTrue();
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        public Booking? StoredBooking { get; private set; }
        public bool SaveChangesCalled { get; private set; }

        public Task AddAsync(Booking booking, CancellationToken cancellationToken)
        {
            StoredBooking = booking;
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeOfferTokenService(OfferSnapshot snapshot) : IOfferTokenService
    {
        public string CreateToken(OfferSnapshot snapshot) => "offer-token";

        public bool TryReadToken(string token, out OfferSnapshot? parsedSnapshot)
        {
            parsedSnapshot = snapshot;
            return token == "offer-token";
        }
    }
}