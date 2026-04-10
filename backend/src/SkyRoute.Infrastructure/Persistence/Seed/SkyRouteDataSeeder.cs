using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Seed;

public sealed class SkyRouteDataSeeder(
    SkyRouteDbContext dbContext,
    IPasswordHasherService passwordHasherService)
{
    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        if (!await dbContext.Airports.AnyAsync(cancellationToken))
        {
            dbContext.Airports.AddRange(
                new Airport { Code = "EZE", Name = "Ministro Pistarini International Airport", City = "Buenos Aires", CountryCode = "AR", CountryName = "Argentina" },
                new Airport { Code = "AEP", Name = "Jorge Newbery Airfield", City = "Buenos Aires", CountryCode = "AR", CountryName = "Argentina" },
                new Airport { Code = "COR", Name = "Ingeniero Aeronautico Ambrosio Taravella Airport", City = "Cordoba", CountryCode = "AR", CountryName = "Argentina" },
                new Airport { Code = "MDZ", Name = "El Plumerillo International Airport", City = "Mendoza", CountryCode = "AR", CountryName = "Argentina" },
                new Airport { Code = "SCL", Name = "Arturo Merino Benitez International Airport", City = "Santiago", CountryCode = "CL", CountryName = "Chile" },
                new Airport { Code = "GRU", Name = "Sao Paulo Guarulhos International Airport", City = "Sao Paulo", CountryCode = "BR", CountryName = "Brazil" });
        }

        if (!await dbContext.Providers.AnyAsync(cancellationToken))
        {
            dbContext.Providers.AddRange(
                new ProviderDefinition { Code = "GLOBAL_AIR", DisplayName = "GlobalAir" },
                new ProviderDefinition { Code = "BUDGET_WINGS", DisplayName = "BudgetWings" });
        }

        if (!await dbContext.Users.AnyAsync(user => user.Email == "demo@skyroute.local", cancellationToken))
        {
            var demoUser = new AppUser
            {
                FullName = "SkyRoute Demo",
                Email = "demo@skyroute.local"
            };

            demoUser.PasswordHash = passwordHasherService.HashPassword(demoUser, "Travel123!");
            dbContext.Users.Add(demoUser);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}