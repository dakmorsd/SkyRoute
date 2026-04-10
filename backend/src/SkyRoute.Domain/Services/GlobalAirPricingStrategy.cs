namespace SkyRoute.Domain.Services;

public sealed class GlobalAirPricingStrategy : IFlightPricingStrategy
{
    public string ProviderCode => "GLOBAL_AIR";

    public decimal CalculatePerPassengerPrice(decimal baseFare)
    {
        return decimal.Round(baseFare * 1.15m, 2, MidpointRounding.AwayFromZero);
    }
}