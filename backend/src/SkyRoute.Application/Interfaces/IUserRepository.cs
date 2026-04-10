using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Interfaces;

public interface IUserRepository
{
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddAsync(AppUser user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}