using LinqToDB;
using LinqToDB.Data;

namespace Infra;

public class MyDatabaseConnection
    (DataOptions<MyDatabaseConnection> options)
    : DataConnection(options.Options), IApplicationData
{
    public ITable<Category> Categories => this.GetTable<Category>();

    public ITable<Item> Items => this.GetTable<Item>();

    public ITable<User> Users => this.GetTable<User>();

    public ITable<Listing> Listings => this.GetTable<Listing>();

    public ITable<Order> Orders => this.GetTable<Order>();

    IQueryable<Category> IApplicationData.Categories => Categories;
    IQueryable<Item> IApplicationData.Items => Items;
    IQueryable<User> IApplicationData.Users => Users;
    IQueryable<Listing> IApplicationData.Listings => Listings;
    IQueryable<Order> IApplicationData.Orders => Orders;

    void IApplicationData.Insert<T>(T entity) => this.Insert(entity);
    void IApplicationData.Update<T>(T entity) => this.Update(entity);
}
