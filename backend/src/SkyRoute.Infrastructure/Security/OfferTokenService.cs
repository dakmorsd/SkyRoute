using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SkyRoute.Application.Interfaces;
using SkyRoute.Application.Models;
using SkyRoute.Infrastructure.Configuration;

namespace SkyRoute.Infrastructure.Security;

public sealed class OfferTokenService(IOptions<OfferTokenOptions> options) : IOfferTokenService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly OfferTokenOptions _options = options.Value;

    public string CreateToken(OfferSnapshot snapshot)
    {
        var payload = JsonSerializer.SerializeToUtf8Bytes(snapshot, SerializerOptions);
        var encodedPayload = Base64UrlEncode(payload);
        var signature = Base64UrlEncode(ComputeSignature(encodedPayload));
        return $"{encodedPayload}.{signature}";
    }

    public bool TryReadToken(string token, out OfferSnapshot? snapshot)
    {
        snapshot = null;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        var parts = token.Split('.');

        if (parts.Length != 2)
        {
            return false;
        }

        var payloadBytes = Base64UrlDecode(parts[0]);
        var providedSignature = Base64UrlDecode(parts[1]);
        var expectedSignature = ComputeSignature(parts[0]);

        if (!CryptographicOperations.FixedTimeEquals(providedSignature, expectedSignature))
        {
            return false;
        }

        snapshot = JsonSerializer.Deserialize<OfferSnapshot>(payloadBytes, SerializerOptions);

        if (snapshot is null)
        {
            return false;
        }

        return snapshot.IssuedAtUtc.AddMinutes(_options.ExpirationMinutes) >= DateTimeOffset.UtcNow;
    }

    private byte[] ComputeSignature(string payload)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.Secret));
        return hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value
            .Replace('-', '+')
            .Replace('_', '/');

        var padding = 4 - (padded.Length % 4);
        if (padding is > 0 and < 4)
        {
            padded = padded.PadRight(padded.Length + padding, '=');
        }

        return Convert.FromBase64String(padded);
    }
}