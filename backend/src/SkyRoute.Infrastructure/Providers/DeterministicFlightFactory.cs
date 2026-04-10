using System.Security.Cryptography;
using System.Text;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Enums;
using SkyRoute.Domain.Services;

namespace SkyRoute.Infrastructure.Providers;

public static class DeterministicFlightFactory
{
    public static IReadOnlyCollection<ProviderFlightOffer> CreateOffers(
        FlightProviderSearchContext context,
        string providerCode,
        string providerName,
        string flightPrefix,
        int providerBias,
        IFlightPricingStrategy pricingStrategy)
    {
        if (context.DepartureDate > DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(180)))
        {
            return Array.Empty<ProviderFlightOffer>();
        }

        var offers = new List<ProviderFlightOffer>();

        for (var index = 0; index < 3; index++)
        {
            var seed = GetSeed($"{providerCode}|{context.Origin.Code}|{context.Destination.Code}|{context.DepartureDate:O}|{context.CabinClass}|{index}");
            var departureDate = context.DepartureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var departureHour = 5 + (seed % 15);
            var departureMinute = (seed / 7 % 4) * 15;
            var departure = new DateTimeOffset(departureDate.AddHours(departureHour).AddMinutes(departureMinute), TimeSpan.Zero);

            var duration = context.RouteType == RouteType.Domestic
                ? 75 + (seed % 165)
                : 165 + (seed % 525);

            var routeBaseFare = context.RouteType == RouteType.Domestic ? 68m : 220m;
            var cabinMultiplier = context.CabinClass switch
            {
                CabinClass.Economy => 1m,
                CabinClass.Business => 1.85m,
                CabinClass.FirstClass => 2.65m,
                _ => 1m
            };

            var baseFare = decimal.Round((routeBaseFare + (seed % 120) + (index * 14m) + providerBias) * cabinMultiplier, 2, MidpointRounding.AwayFromZero);
            var perPassenger = pricingStrategy.CalculatePerPassengerPrice(baseFare);
            var total = decimal.Round(perPassenger * context.PassengerCount, 2, MidpointRounding.AwayFromZero);

            offers.Add(new ProviderFlightOffer(
                providerCode,
                providerName,
                $"{flightPrefix}{1000 + (seed % 8000)}",
                context.Origin.Code,
                context.Origin.CountryCode,
                context.Destination.Code,
                context.Destination.CountryCode,
                departure,
                departure.AddMinutes(duration),
                context.CabinClass,
                context.RouteType,
                perPassenger,
                total));
        }

        return offers;
    }

    private static int GetSeed(string input)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Math.Abs(BitConverter.ToInt32(hash, 0));
    }
}