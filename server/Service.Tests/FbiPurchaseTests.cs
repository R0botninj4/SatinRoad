using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Identity;

namespace Service.Tests;

public class FbiPurchaseTests
{
    [Fact]
    public void FbiPurchase_CompletesOrderAndPermanentlyClosesVendorShop()
    {
        using var db = CreateDatabase();
        db.Insert(NewUser("buyer"));
        var vendor = NewUser("vendor");
        var passwordHasher = new PasswordHasher<User>();
        vendor.PasswordHash = passwordHasher.HashPassword(vendor, "secret");
        db.Insert(vendor);
        db.Insert(NewUser("other-vendor"));
        db.Insert(NewListing("first", "vendor", 5));
        db.Insert(NewListing("second", "vendor", 7));
        db.Insert(NewListing("other", "other-vendor", 3));

        var orderService = new OrderService(db, new FixedFbiCheck(true));
        var order = orderService.Create("buyer", new CreateOrderRequestDto
        {
            ListingId = "first",
            Quantity = 2
        });

        Assert.Equal(20m, order.TotalPrice);
        Assert.Single(db.Orders.ToList());
        Assert.True(db.Users.Single(user => user.Id == "vendor").IsShutDown);
        Assert.True(new UserService(db, new PasswordHasher<User>())
            .GetById("vendor")!.IsShutDown);
        var loggedInVendor = new UserService(db, passwordHasher).Login(
            new LoginRequestDto { Username = "vendor", Password = "secret" });
        Assert.NotNull(loggedInVendor);
        Assert.True(loggedInVendor.IsShutDown);
        Assert.Empty(db.Listings.Where(listing => listing.UserId == "vendor").ToList());
        Assert.Single(db.Listings.Where(listing => listing.UserId == "other-vendor").ToList());

        var listingService = new ListingService(db);
        Assert.Throws<ValidationException>(() => listingService.Create("vendor", new CreateListingRequestDto
        {
            ItemId = 1,
            Price = 10m,
            Quantity = 1
        }));

        var laterOrder = new OrderService(db, new FixedFbiCheck(false)).Create(
            "vendor", new CreateOrderRequestDto { ListingId = "other", Quantity = 1 });
        Assert.Equal("vendor", laterOrder.BuyerId);
        Assert.Equal(2, db.Orders.Count());
    }

    [Fact]
    public void NormalPurchase_KeepsVendorAndOtherListingsOpen()
    {
        using var db = CreateDatabase();
        db.Insert(NewUser("buyer"));
        db.Insert(NewUser("vendor"));
        db.Insert(NewListing("first", "vendor", 5));
        db.Insert(NewListing("second", "vendor", 7));

        new OrderService(db, new FixedFbiCheck(false)).Create(
            "buyer", new CreateOrderRequestDto { ListingId = "first", Quantity = 2 });

        Assert.False(db.Users.Single(user => user.Id == "vendor").IsShutDown);
        Assert.Equal(3, db.Listings.Single(listing => listing.Id == "first").Quantity);
        Assert.Single(db.Listings.Where(listing => listing.Id == "second").ToList());
    }

    [Fact]
    public void InvalidPurchase_DoesNotCloseVendorOrCreateOrder()
    {
        using var db = CreateDatabase();
        db.Insert(NewUser("buyer"));
        db.Insert(NewUser("vendor"));
        db.Insert(NewListing("first", "vendor", 1));

        Assert.Throws<ValidationException>(() =>
            new OrderService(db, new FixedFbiCheck(true)).Create(
                "buyer", new CreateOrderRequestDto { ListingId = "first", Quantity = 2 }));

        Assert.False(db.Users.Single(user => user.Id == "vendor").IsShutDown);
        Assert.Empty(db.Orders.ToList());
        Assert.Single(db.Listings.ToList());
    }

    private static MyDatabaseConnection CreateDatabase()
    {
        var options = new DataOptions().UseSQLite("Data Source=:memory:");
        var db = new MyDatabaseConnection(new DataOptions<MyDatabaseConnection>(options));
        db.CreateTable<User>();
        db.CreateTable<Listing>();
        db.CreateTable<Order>();
        return db;
    }

    private static User NewUser(string id) => new()
    {
        Id = id,
        Username = id,
        NormalizedUsername = id.ToUpperInvariant(),
        PasswordHash = "test"
    };

    private static Listing NewListing(string id, string vendorId, int quantity) => new()
    {
        Id = id,
        UserId = vendorId,
        ItemId = 1,
        Price = 10m,
        Quantity = quantity,
        Description = "test"
    };

    private sealed class FixedFbiCheck(bool isFbiBuyer) : IFbiCheck
    {
        public bool IsFbiBuyer() => isFbiBuyer;
    }
}
