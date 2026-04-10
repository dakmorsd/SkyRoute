using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SkyRoute.Application.Interfaces;
using SkyRoute.Infrastructure.Persistence;
using SkyRoute.Infrastructure.Providers;

namespace SkyRoute.Api.IntegrationTests;

public sealed class SkyRouteApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"skyroute-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<SkyRouteDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<SkyRouteDbContext>>();
            services.RemoveAll<SkyRouteDbContext>();

            services.AddDbContext<SkyRouteDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<GlobalAirProviderClient>();
            services.RemoveAll<BudgetWingsProviderClient>();
            services.RemoveAll<IFlightProviderClient>();

            services.AddScoped(sp =>
            {
                var client = CreateClient();
                client.BaseAddress = new Uri(client.BaseAddress!, "mock/globalair/");
                return new GlobalAirProviderClient(client);
            });

            services.AddScoped(sp =>
            {
                var client = CreateClient();
                client.BaseAddress = new Uri(client.BaseAddress!, "mock/budgetwings/");
                return new BudgetWingsProviderClient(client);
            });

            services.AddScoped<IFlightProviderClient>(sp => sp.GetRequiredService<GlobalAirProviderClient>());
            services.AddScoped<IFlightProviderClient>(sp => sp.GetRequiredService<BudgetWingsProviderClient>());
        });
    }
}