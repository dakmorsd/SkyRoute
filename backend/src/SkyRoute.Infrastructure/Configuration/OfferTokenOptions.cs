namespace SkyRoute.Infrastructure.Configuration;

public sealed class OfferTokenOptions
{
    public const string SectionName = "OfferTokens";

    public string Secret { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 120;
}