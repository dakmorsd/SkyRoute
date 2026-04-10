namespace SkyRoute.Domain.Services;

public sealed class BudgetWingsPricingStrategy : IFlightPricingStrategy
{
    private const decimal MinimumPrice = 29.99m;

    public string ProviderCode => "BUDGET_WINGS";

    public decimal CalculatePerPassengerPrice(decimal baseFare)
    {
        var discounted = decimal.Round(baseFare * 0.9m, 2, MidpointRounding.AwayFromZero);
        return discounted < MinimumPrice ? MinimumPrice : discounted;
    }
}