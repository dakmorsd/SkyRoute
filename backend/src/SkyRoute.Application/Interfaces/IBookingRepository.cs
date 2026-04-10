using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}