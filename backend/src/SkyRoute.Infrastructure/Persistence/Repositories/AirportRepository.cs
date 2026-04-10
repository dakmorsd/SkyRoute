using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Repositories;

public sealed class AirportRepository(SkyRouteDbContext dbContext) : IAirportRepository
{
    public async Task<IReadOnlyCollection<Airport>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Airports
            .AsNoTracking()
            .OrderBy(airport => airport.CountryName)
            .ThenBy(airport => airport.City)
            .ToListAsync(cancellationToken);
    }

    public async Task<Airport?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await dbContext.Airports
            .AsNoTracking()
            .FirstOrDefaultAsync(airport => airport.Code == code, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Airport>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken)
    {
        var normalizedCodes = codes.Select(code => code.ToUpperInvariant()).ToArray();

        return await dbContext.Airports
            .AsNoTracking()
            .Where(airport => normalizedCodes.Contains(airport.Code))
            .ToListAsync(cancellationToken);
    }
}