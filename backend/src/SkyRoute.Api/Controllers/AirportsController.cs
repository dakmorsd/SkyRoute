using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.DTOs.Airports;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AirportsController(IAirportQueryService airportQueryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<AirportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AirportDto>>> GetAll(CancellationToken cancellationToken)
    {
        var airports = await airportQueryService.GetAllAsync(cancellationToken);
        return Ok(airports);
    }
}