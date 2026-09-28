using LinqToDB;
using LinqToDB.Data;

namespace Infra;

public class MyDatabaseConnection
    (DataOptions<MyDatabaseConnection> options)
    : DataConnection(options.Options)
{
    public ITable<Category> Categories => this.GetTable<Category>();

    public ITable<Item> Items => this.GetTable<Item>();

    public ITable<User> Users => this.GetTable<User>();

    public ITable<Listing> Listings => this.GetTable<Listing>();
}