using Microsoft.AspNetCore.Identity;
using SkyRoute.Application.Interfaces;
using SkyRoute.Domain.Entities;

namespace SkyRoute.Infrastructure.Security;

public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    public string HashPassword(AppUser user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(AppUser user, string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}