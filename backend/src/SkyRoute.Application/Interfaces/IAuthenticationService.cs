using SkyRoute.Application.DTOs.Auth;

namespace SkyRoute.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}