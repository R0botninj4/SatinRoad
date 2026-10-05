using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Service.Tests;

public class OrderHelpersTests
{
    [Fact]
    public void CalculateTotalPrice_ThreeItems_ReturnsTotalPrice()
    {
        // Arrange
        decimal price = 12.50m;
        int quantity = 3;

        // Act
        decimal result = OrderHelpers.CalculateTotalPrice(price, quantity);

        // Assert
        Assert.Equal(37.50m, result);
    }

    [Theory]
    [InlineData(10, 1, 10)]
    [InlineData(5, 4, 20)]
    [InlineData(0.10, 3, 0.30)]
    public void CalculateTotalPrice_VariousAmounts_ReturnsExpectedPrice(
        decimal price, int quantity, decimal expectedPrice)
    {
        // Act
        decimal result = OrderHelpers.CalculateTotalPrice(price, quantity);

        // Assert
        Assert.Equal(expectedPrice, result);
    }

    [Theory]
    [InlineData(10, 3, 7)]
    [InlineData(10, 10, 0)]
    [InlineData(1, 1, 0)]
    public void CalculateRemainingStock_EnoughStock_ReturnsRemainingStock(
        int stock, int quantity, int expectedStock)
    {
        // Act
        int result = OrderHelpers.CalculateRemainingStock(stock, quantity);

        // Assert
        Assert.Equal(expectedStock, result);
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(0, 1)]
    public void CalculateRemainingStock_NotEnoughStock_ThrowsException(int stock, int quantity)
    {
        // Act and Assert
        Assert.Throws<ValidationException>(() => OrderHelpers.CalculateRemainingStock(stock, quantity));
    }
}
