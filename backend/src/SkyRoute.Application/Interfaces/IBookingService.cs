using SkyRoute.Application.DTOs.Bookings;
using SkyRoute.Application.Models;

namespace SkyRoute.Application.Interfaces;

public interface IBookingService
{
    Task<BookingResponse> BookAsync(
        BookingRequest request,
        AuthenticatedUser authenticatedUser,
        CancellationToken cancellationToken);
}