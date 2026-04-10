using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Domain.Entities;
using SkyRoute.Infrastructure.Configuration;

namespace SkyRoute.Infrastructure.Security;

public sealed class JwtTokenService(IOptions<JwtOptions> jwtOptions) : IJwtTokenService
{
    private readonly JwtOptions _options = jwtOptions.Value;

    public AuthToken IssueToken(AppUser user)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_options.ExpirationMinutes);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Email, user.Email)
        };

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        var tokenHandler = new JwtSecurityTokenHandler();
        return new AuthToken(tokenHandler.WriteToken(tokenDescriptor), expiresAt);
    }
}