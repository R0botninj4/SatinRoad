using System.ComponentModel.DataAnnotations;

namespace Service;

public static class OrderHelpers
{
    public static decimal CalculateTotalPrice(
        decimal price,
        int quantity,
        int previousOrders = 0)
    {
        var total = price * quantity;

        if (HasLoyaltyDiscount(previousOrders))
        {
            return decimal.Round(total * MarketplaceRules.LoyaltyDiscountMultiplier, 2);
        }

        return total;
    }

    public static int CalculateRemainingStock(int stock, int quantity)
    {
        if (stock < quantity)
        {
            throw new ValidationException("Not enough items in stock.");
        }

        return stock - quantity;
    }
    
    public static bool HasLoyaltyDiscount(int previousOrders)
    {
        return (previousOrders + 1) % MarketplaceRules.LoyaltyDiscountInterval == 0;
    }
}
