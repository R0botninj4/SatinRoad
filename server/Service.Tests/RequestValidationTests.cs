using System.ComponentModel.DataAnnotations;

namespace Service.Tests;

// ASP.NET validates these annotations before invoking controller actions.
// Service tests alone do not exercise that boundary.
public class RequestValidationTests
{
    private static bool IsValid(object request) => Validator.TryValidateObject(
        request, new ValidationContext(request), new List<ValidationResult>(), validateAllProperties: true);

    [Theory]
    [InlineData(0, false)]
    [InlineData(-1, false)]
    [InlineData(1, true)]
    public void Purchase_quantity_must_be_positive(int quantity, bool valid) =>
        Assert.Equal(valid, IsValid(new CreateOrderRequestDto { ListingId = "listing", Quantity = quantity }));

    [Fact]
    public void Purchase_requires_listing_id() =>
        Assert.False(IsValid(new CreateOrderRequestDto { ListingId = "", Quantity = 1 }));

    [Theory]
    [InlineData(0, 1, 1, false)]
    [InlineData(1, 0, 1, false)]
    [InlineData(1, -1, 1, false)]
    [InlineData(1, 1, 0, false)]
    [InlineData(1, 1, -1, false)]
    [InlineData(1, 0.01, 1, true)]
    public void Listing_requires_item_positive_price_and_positive_stock(int itemId, double price, int quantity, bool valid) =>
        Assert.Equal(valid, IsValid(new CreateListingRequestDto { ItemId = itemId, Price = (decimal)price, Quantity = quantity }));

    [Theory]
    [InlineData("ab", "CorrectPassword123!", false)]
    [InlineData("Alice", "short", false)]
    [InlineData("Alice", "CorrectPassword123!", true)]
    public void Registration_requires_valid_username_and_password_length(string username, string password, bool valid) =>
        Assert.Equal(valid, IsValid(new RegisterRequestDto { Username = username, Password = password }));
}
