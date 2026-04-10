using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Interfaces;

public interface IAirportRepository
{
    Task<IReadOnlyCollection<Airport>> GetAllAsync(CancellationToken cancellationToken);
    Task<Airport?> GetByCodeAsync(string code, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Airport>> GetByCodesAsync(IEnumerable<string> codes, CancellationToken cancellationToken);
}