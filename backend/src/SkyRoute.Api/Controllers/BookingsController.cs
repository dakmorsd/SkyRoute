using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.DTOs.Bookings;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(BookingResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResponse>> Book(BookingRequest request, CancellationToken cancellationToken)
    {
        var authenticatedUser = new AuthenticatedUser(
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
            User.FindFirstValue(ClaimTypes.Name)!,
            User.FindFirstValue(ClaimTypes.Email)!);

        var response = await bookingService.BookAsync(request, authenticatedUser, cancellationToken);
        return Ok(response);
    }
}