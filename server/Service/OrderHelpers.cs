using System.ComponentModel.DataAnnotations;

namespace Service;

public static class OrderHelpers
{
    public static decimal CalculateTotalPrice(decimal price, int quantity)
    {
        return price * quantity;
    }

    public static int CalculateRemainingStock(int stock, int quantity)
    {
        if (stock < quantity)
        {
            throw new ValidationException("Not enough items in stock.");
        }

        return stock - quantity;
    }
}
