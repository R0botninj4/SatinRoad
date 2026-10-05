using Infra;
using LinqToDB;

namespace Service.Tests;

public class ItemServiceTests : TestDatabase
{
    [Fact]
    public void Products_are_returned_in_id_order()
    {
        Db.Insert(new Item { Id = 2, Name = "Seeds" });
        Db.Insert(new Item { Id = 1, Name = "Wheat" });
        Assert.Equal(new[] { "Wheat", "Seeds" }, new ItemService(Db).GetAll().Select(item => item.Name));
    }

    [Fact]
    public void Empty_catalog_returns_empty_list() => Assert.Empty(new ItemService(Db).GetAll());
}
