using SkyRoute.Domain.Entities;

namespace SkyRoute.Application.Interfaces;

public interface IPasswordHasherService
{
    string HashPassword(AppUser user, string password);
    bool VerifyPassword(AppUser user, string hashedPassword, string password);
}