using Infra;
using LinqToDB;
using LinqToDB.Data;

namespace Service.Tests;

public class ListingServiceTests
{
    [Fact]
    public void GetAll_FeaturesVendorOnlyAfterMoreThanOneHundredSales()
    {
        // Arrange
        var options = new DataOptions().UseSQLite("Data Source=:memory:");
        using var db = new MyDatabaseConnection(new DataOptions<MyDatabaseConnection>(options));
        db.CreateTable<User>();
        db.CreateTable<Listing>();
        db.CreateTable<Order>();

        db.Insert(new User { Id = "vendor", Username = "Vendor" });
        db.Insert(new User { Id = "other", Username = "Other" });
        db.Insert(new Listing { Id = "vendor-listing", UserId = "vendor", Price = 10m });
        db.Insert(new Listing { Id = "other-listing", UserId = "other", Price = 1m });

        for (var i = 0; i < 100; i++)
        {
            db.Insert(new Order { Id = $"order-{i}", SellerId = "vendor" });
        }

        var service = new ListingService(db);

        // Act and assert: 100 sales are not enough.
        Assert.All(service.GetAll(), listing => Assert.False(listing.IsFeatured));

        db.Insert(new Order { Id = "order-100", SellerId = "vendor" });
        var listings = service.GetAll();

        // The featured vendor comes first even though the other listing is cheaper.
        Assert.Equal("vendor-listing", listings[0].Id);
        Assert.Equal("Vendor", listings[0].Username);
        Assert.True(listings[0].IsFeatured);
        Assert.False(listings[1].IsFeatured);
    }
}
