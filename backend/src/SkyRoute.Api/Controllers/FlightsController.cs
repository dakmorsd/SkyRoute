using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.DTOs.Flights;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FlightsController(IFlightSearchService flightSearchService) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(FlightSearchResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<FlightSearchResponse>> Search(FlightSearchRequest request, CancellationToken cancellationToken)
    {
        var response = await flightSearchService.SearchAsync(request, cancellationToken);
        return Ok(response);
    }
}