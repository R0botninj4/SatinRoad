using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service.Tests;

public class ListingServiceTests : TestDatabase
{
    [Fact]
    public void Creating_listing_persists_owner_price_stock_and_trimmed_description()
    {
        AddUser();
        Db.Insert(new Item { Id = 1, Name = "Wheat" });
        var listing = new ListingService(Db).Create("vendor", new()
        {
            ItemId = 1, Price = 4.25m, Quantity = 7, Description = "  Fresh wheat  "
        });

        var saved = Db.Listings.Single();
        Assert.Equal(listing.Id, saved.Id);
        Assert.Equal("vendor", saved.UserId);
        Assert.Equal(1, saved.ItemId);
        Assert.Equal(4.25m, saved.Price);
        Assert.Equal(7, saved.Quantity);
        Assert.Equal("Fresh wheat", saved.Description);
    }

    [Theory]
    [InlineData("missing", 1)]
    [InlineData("vendor", 999)]
    public void Creating_listing_rejects_missing_user_or_item_without_inserting(string userId, int itemId)
    {
        AddUser();
        Db.Insert(new Item { Id = 1, Name = "Wheat" });
        Assert.Throws<ValidationException>(() => new ListingService(Db).Create(userId,
            new() { ItemId = itemId, Price = 1, Quantity = 1 }));
        Assert.Empty(Db.Listings.ToList());
    }

    [Fact]
    public void Listings_include_vendor_name_and_current_stock()
    {
        AddListing();
        var listing = Assert.Single(new ListingService(Db).GetAll());
        Assert.Equal("Vendor", listing.Username);
        Assert.Equal("vendor", listing.UserId);
        Assert.Equal(10, listing.Quantity);
        Assert.Equal(12.50m, listing.Price);
    }

    [Fact]
    public void Listing_with_missing_vendor_uses_fallback_name()
    {
        AddListing();
        Db.Users.Where(user => user.Id == "vendor").Delete();
        Assert.Equal("Unknown vendor", Assert.Single(new ListingService(Db).GetAll()).Username);
    }
}
