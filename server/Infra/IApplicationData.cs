namespace Infra;

// Services depend on this boundary; production uses Linq2db and tests use a fake.
public interface IApplicationData
{
    IQueryable<Category> Categories { get; }
    IQueryable<Item> Items { get; }
    IQueryable<User> Users { get; }
    IQueryable<Listing> Listings { get; }
    IQueryable<Order> Orders { get; }
    void Insert<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
}
