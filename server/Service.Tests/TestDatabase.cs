using Infra;
using LinqToDB;
using LinqToDB.Data;

namespace Service.Tests;

// Each test owns a real SQLite database; no application database is accessed.
public abstract class TestDatabase : IDisposable
{
    private readonly string path = Path.Combine(Path.GetTempPath(), $"satinroad-test-{Guid.NewGuid():N}.db");
    protected MyDatabaseConnection Db { get; }

    protected TestDatabase()
    {
        var options = new DataOptions().UseSQLite($"Data Source={path};Pooling=False");
        Db = new MyDatabaseConnection(new DataOptions<MyDatabaseConnection>(options));
        Db.CreateTable<User>();
        Db.Execute("CREATE UNIQUE INDEX IX_Users_NormalizedUsername ON Users (NormalizedUsername)");
        Db.CreateTable<Category>();
        Db.CreateTable<Item>();
        Db.CreateTable<Listing>();
        Db.CreateTable<Order>();
    }

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

    public void Dispose()
    {
        Db.Dispose();
        File.Delete(path);
    }
}
