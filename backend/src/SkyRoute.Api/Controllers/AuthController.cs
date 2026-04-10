using Microsoft.AspNetCore.Mvc;
using SkyRoute.Application.DTOs.Auth;
using SkyRoute.Application.Interfaces;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.RegisterAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await authenticationService.LoginAsync(request, cancellationToken);
        return Ok(response);
    }
}