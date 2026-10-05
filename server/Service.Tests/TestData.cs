using Infra;

namespace Service.Tests;

public abstract class TestData
{
    protected FakeApplicationData Db { get; } = new();

    protected User AddUser(string id = "vendor", string username = "Vendor")
    {
        var user = new User { Id = id, Username = username, NormalizedUsername = username.ToUpperInvariant() };
        Db.Insert(user);
        return user;
    }

    protected Listing AddListing(int quantity = 10, decimal price = 12.50m)
    {
        AddUser();
        AddUser("buyer", "Buyer");
        Db.Insert(new Item { Id = 1, Name = "Wheat" });
        var listing = new Listing { Id = "listing", UserId = "vendor", ItemId = 1, Quantity = quantity, Price = price };
        Db.Insert(listing);
        return listing;
    }
}
