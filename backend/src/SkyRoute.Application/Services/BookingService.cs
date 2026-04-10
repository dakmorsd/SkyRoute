using System.Net.Mail;
using System.Text.RegularExpressions;
using SkyRoute.Application.DTOs.Bookings;
using SkyRoute.Application.Exceptions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Services;

namespace SkyRoute.Application.Services;

public sealed class BookingService(
    IBookingRepository bookingRepository,
    IOfferTokenService offerTokenService) : IBookingService
{
    public async Task<BookingResponse> BookAsync(
        BookingRequest request,
        AuthenticatedUser authenticatedUser,
        CancellationToken cancellationToken)
    {
        if (!offerTokenService.TryReadToken(request.OfferToken, out var snapshot) || snapshot is null)
        {
            throw new RequestValidationException(new Dictionary<string, string[]>
            {
                [nameof(request.OfferToken)] = ["The selected flight offer is invalid or expired."]
            });
        }

        ValidatePassengers(request, snapshot);

        var documentRule = DocumentRuleFactory.Create(snapshot.RouteType);

        var booking = new Booking
        {
            ReferenceCode = GenerateReferenceCode(),
            UserId = authenticatedUser.Id,
            ProviderCode = snapshot.ProviderCode,
            ProviderDisplayName = snapshot.ProviderName,
            FlightNumber = snapshot.FlightNumber,
            OriginAirportCode = snapshot.OriginAirportCode,
            OriginCountryCode = snapshot.OriginCountryCode,
            DestinationAirportCode = snapshot.DestinationAirportCode,
            DestinationCountryCode = snapshot.DestinationCountryCode,
            DepartureTimeUtc = snapshot.DepartureTimeUtc,
            ArrivalTimeUtc = snapshot.ArrivalTimeUtc,
            CabinClass = snapshot.CabinClass,
            RouteType = snapshot.RouteType,
            PassengerCount = snapshot.PassengerCount,
            PerPassengerPrice = snapshot.PerPassengerPrice,
            TotalPrice = snapshot.TotalPrice,
            Passengers = request.Passengers.Select(passenger => new BookingPassenger
            {
                FullName = passenger.FullName.Trim(),
                Email = passenger.Email.Trim().ToLowerInvariant(),
                DocumentType = documentRule.Type,
                DocumentNumber = passenger.DocumentNumber.Trim().ToUpperInvariant()
            }).ToList()
        };

        await bookingRepository.AddAsync(booking, cancellationToken);
        await bookingRepository.SaveChangesAsync(cancellationToken);

        return new BookingResponse(
            booking.ReferenceCode,
            booking.ProviderCode,
            booking.FlightNumber,
            booking.RouteType,
            booking.PassengerCount,
            booking.PerPassengerPrice,
            booking.TotalPrice);
    }

    private static string GenerateReferenceCode()
    {
        return $"SKY-{Guid.NewGuid():N}"[..12].ToUpperInvariant();
    }

    private static void ValidatePassengers(BookingRequest request, OfferSnapshot snapshot)
    {
        var errors = new Dictionary<string, string[]>();

        if (request.Passengers is null || request.Passengers.Count == 0)
        {
            errors[nameof(request.Passengers)] = ["At least one passenger is required."];
            throw new RequestValidationException(errors);
        }

        if (request.Passengers.Count != snapshot.PassengerCount)
        {
            errors[nameof(request.Passengers)] = [$"Exactly {snapshot.PassengerCount} passenger entries are required."];
        }

        var rule = DocumentRuleFactory.Create(snapshot.RouteType);

        for (var index = 0; index < request.Passengers.Count; index++)
        {
            var passenger = request.Passengers.ElementAt(index);
            var prefix = $"Passengers[{index}]";

            if (string.IsNullOrWhiteSpace(passenger.FullName))
            {
                errors[$"{prefix}.FullName"] = ["Full name is required."];
            }

            if (!IsValidEmail(passenger.Email))
            {
                errors[$"{prefix}.Email"] = ["A valid email address is required."];
            }

            if (string.IsNullOrWhiteSpace(passenger.DocumentNumber) ||
                !Regex.IsMatch(passenger.DocumentNumber.Trim().ToUpperInvariant(), rule.Pattern, RegexOptions.CultureInvariant))
            {
                errors[$"{prefix}.DocumentNumber"] = [$"{rule.Label} is invalid."];
            }
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}