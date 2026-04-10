using Microsoft.EntityFrameworkCore;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(SkyRouteDbContext dbContext) : IUserRepository
{
    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        return dbContext.Users.AddAsync(user, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}