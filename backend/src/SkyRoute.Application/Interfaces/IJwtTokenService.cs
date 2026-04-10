using SkyRoute.Application.Models;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Interfaces;

public interface IJwtTokenService
{
    AuthToken IssueToken(AppUser user);
}