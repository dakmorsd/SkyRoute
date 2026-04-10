using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(SkyRouteDbContext dbContext) : IBookingRepository
{
    public Task AddAsync(Booking booking, CancellationToken cancellationToken)
    {
        return dbContext.Bookings.AddAsync(booking, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}