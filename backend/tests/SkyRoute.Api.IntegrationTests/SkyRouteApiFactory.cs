using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SkyRoute.Infrastructure.Persistence;

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
        });
    }
}