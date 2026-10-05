using System.ComponentModel.DataAnnotations;

namespace Service.Tests;

public class OrderServiceTests : TestData
{
    [Theory]
    [InlineData(1, 9, 12.50)]
    [InlineData(3, 7, 37.50)]
    [InlineData(10, 0, 125.00)]
    public void Purchase_creates_order_calculates_total_and_updates_stock(int quantity, int remaining, double total)
    {
        AddListing();
        var insertCalls = Db.InsertCalls;
        var updateCalls = Db.UpdateCalls;
        var before = DateTime.UtcNow;
        var order = new OrderService(Db).Create("buyer", new() { ListingId = "listing", Quantity = quantity });

        var saved = Assert.Single(Db.Orders.ToList());
        Assert.Equal(order.Id, saved.Id);
        Assert.Equal("buyer", saved.BuyerId);
        Assert.Equal("listing", saved.ListingId);
        Assert.Equal(quantity, saved.Quantity);
        Assert.Equal((decimal)total, saved.TotalPrice);
        Assert.InRange(order.CreatedAt, before, DateTime.UtcNow);
        Assert.Equal(remaining, Db.Listings.Single().Quantity);
        Assert.Equal(insertCalls + 1, Db.InsertCalls);
        Assert.Equal(updateCalls + 1, Db.UpdateCalls);
    }

    [Theory]
    [InlineData("missing", "listing", 1)]
    [InlineData("buyer", "missing", 1)]
    [InlineData("buyer", "listing", 11)]
    public void Rejected_purchase_does_not_create_order_or_change_stock(string buyerId, string listingId, int quantity)
    {
        AddListing();
        var insertCalls = Db.InsertCalls;
        var updateCalls = Db.UpdateCalls;
        Assert.Throws<ValidationException>(() => new OrderService(Db).Create(buyerId,
            new() { ListingId = listingId, Quantity = quantity }));

        Assert.Empty(Db.Orders.ToList());
        Assert.Equal(10, Db.Listings.Single().Quantity);
        Assert.Equal(insertCalls, Db.InsertCalls);
        Assert.Equal(updateCalls, Db.UpdateCalls);
    }

    [Fact]
    public void Fractional_unit_price_uses_exact_decimal_total()
    {
        AddListing(price: 0.10m);
        var order = new OrderService(Db).Create("buyer", new() { ListingId = "listing", Quantity = 3 });
        Assert.Equal(0.30m, order.TotalPrice);
    }

    [Fact]
    public void Sold_out_listing_rejects_another_purchase()
    {
        AddListing(quantity: 1);
        var service = new OrderService(Db);
        service.Create("buyer", new() { ListingId = "listing", Quantity = 1 });

        Assert.Throws<ValidationException>(() => service.Create("buyer", new() { ListingId = "listing", Quantity = 1 }));
        Assert.Single(Db.Orders.ToList());
        Assert.Equal(0, Db.Listings.Single().Quantity);
    }

    [Fact]
    public void Order_history_only_returns_buyers_orders_newest_first()
    {
        AddListing();
        var service = new OrderService(Db);
        var older = service.Create("buyer", new() { ListingId = "listing", Quantity = 1 });
        older.CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Db.Update(older);
        service.Create("vendor", new() { ListingId = "listing", Quantity = 1 });
        var newer = service.Create("buyer", new() { ListingId = "listing", Quantity = 2 });

        var orders = service.GetMyOrders("buyer");
        Assert.Equal(new[] { newer.Id, older.Id }, orders.Select(order => order.Id));
        Assert.All(orders, order => Assert.Equal("buyer", order.BuyerId));
        Assert.Empty(service.GetMyOrders("missing"));
    }
}
