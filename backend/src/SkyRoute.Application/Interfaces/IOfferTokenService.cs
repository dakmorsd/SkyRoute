using SkyRoute.Application.Models;

namespace SkyRoute.Application.Interfaces;

public interface IOfferTokenService
{
    string CreateToken(OfferSnapshot snapshot);
    bool TryReadToken(string token, out OfferSnapshot? snapshot);
}