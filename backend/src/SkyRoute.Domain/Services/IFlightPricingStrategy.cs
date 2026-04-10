namespace SkyRoute.Domain.Services;

public interface IFlightPricingStrategy
{
    string ProviderCode { get; }

    decimal CalculatePerPassengerPrice(decimal baseFare);
}