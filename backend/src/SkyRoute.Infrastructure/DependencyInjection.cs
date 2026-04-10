using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Services;
using SkyRoute.Infrastructure.Configuration;
using SkyRoute.Infrastructure.Persistence;
using SkyRoute.Infrastructure.Persistence.Repositories;
using SkyRoute.Infrastructure.Persistence.Seed;
using SkyRoute.Infrastructure.Providers;
using SkyRoute.Infrastructure.Security;

namespace SkyRoute.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SkyRoute")
            ?? throw new InvalidOperationException("The SkyRoute connection string is missing.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<OfferTokenOptions>(configuration.GetSection(OfferTokenOptions.SectionName));

        services.AddDbContext<SkyRouteDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(SkyRouteDbContext).Assembly.FullName)));

        services.AddScoped<IAirportRepository, AirportRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        services.AddScoped<IPasswordHasherService, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOfferTokenService, OfferTokenService>();

        services.AddSingleton<GlobalAirPricingStrategy>();
        services.AddSingleton<BudgetWingsPricingStrategy>();

        services.AddScoped<SkyRouteDataSeeder>();

        return services;
    }

    /// <summary>
    /// Registers the airline provider HTTP clients.
    /// Called from Program.cs after the server URLs are known, so each client
    /// points to the corresponding mock provider endpoint hosted in the same process.
    /// </summary>
    public static IServiceCollection AddProviderClients(this IServiceCollection services, string serverBaseUrl)
    {
        var baseUri = serverBaseUrl.TrimEnd('/');

        services.AddHttpClient<GlobalAirProviderClient>(client =>
        {
            client.BaseAddress = new Uri($"{baseUri}/mock/globalair/");
        });

        services.AddHttpClient<BudgetWingsProviderClient>(client =>
        {
            client.BaseAddress = new Uri($"{baseUri}/mock/budgetwings/");
        });

        services.AddScoped<IFlightProviderClient>(sp => sp.GetRequiredService<GlobalAirProviderClient>());
        services.AddScoped<IFlightProviderClient>(sp => sp.GetRequiredService<BudgetWingsProviderClient>());

        return services;
    }
}