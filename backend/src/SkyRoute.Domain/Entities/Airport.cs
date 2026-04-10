namespace SkyRoute.Domain.Entities;

public sealed class Airport
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
}