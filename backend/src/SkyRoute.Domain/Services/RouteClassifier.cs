using SkyRoute.Domain.Entities;
using SkyRoute.Domain.Enums;

namespace SkyRoute.Domain.Services;

public static class RouteClassifier
{
    public static RouteType Classify(Airport origin, Airport destination)
    {
        return origin.CountryCode.Equals(destination.CountryCode, StringComparison.OrdinalIgnoreCase)
            ? RouteType.Domestic
            : RouteType.International;
    }
}