using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Services;

namespace SkyRoute.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IAirportQueryService, AirportQueryService>();
        services.AddScoped<IFlightSearchService, FlightSearchService>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }
}