using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Service.Tests;

public class RequestValidationTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    public void OrderQuantity_VariousAmounts_ReturnsExpectedValidation(int quantity, bool expected)
    {
        // Arrange
        var request = new CreateOrderRequestDto { ListingId = "listing", Quantity = quantity };
        var errors = new List<ValidationResult>();

        // Act
        bool result = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void OrderListingId_EmptyId_IsInvalid()
    {
        // Arrange
        var request = new CreateOrderRequestDto { ListingId = "", Quantity = 1 };
        var errors = new List<ValidationResult>();

        // Act
        bool result = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0, 1, 1, false)]
    [InlineData(1, 0, 1, false)]
    [InlineData(1, -1, 1, false)]
    [InlineData(1, 1, 0, false)]
    [InlineData(1, 1, -1, false)]
    [InlineData(1, 0.01, 1, true)]
    public void Listing_VariousInputs_ReturnsExpectedValidation(
        int itemId, decimal price, int quantity, bool expected)
    {
        // Arrange
        var request = new CreateListingRequestDto { ItemId = itemId, Price = price, Quantity = quantity };
        var errors = new List<ValidationResult>();

        // Act
        bool result = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("ab", "CorrectPassword123!", false)]
    [InlineData("Alice", "short", false)]
    [InlineData("Alice", "CorrectPassword123!", true)]
    public void Registration_VariousInputs_ReturnsExpectedValidation(
        string username, string password, bool expected)
    {
        // Arrange
        var request = new RegisterRequestDto { Username = username, Password = password };
        var errors = new List<ValidationResult>();

        // Act
        bool result = Validator.TryValidateObject(request, new ValidationContext(request), errors, true);

        // Assert
        Assert.Equal(expected, result);
    }
}
