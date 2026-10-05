using Infra;

namespace Service.Tests;

// Copies prevent changes to a queried entity from silently replacing an Update call.
public class FakeApplicationData : IApplicationData
{
    private readonly Dictionary<Type, List<object>> rows = new();
    public int InsertCalls { get; private set; }
    public int UpdateCalls { get; private set; }

    public IQueryable<Category> Categories => Query<Category>();
    public IQueryable<Item> Items => Query<Item>();
    public IQueryable<User> Users => Query<User>();
    public IQueryable<Listing> Listings => Query<Listing>();
    public IQueryable<Order> Orders => Query<Order>();

    private List<object> Rows<T>() where T : class
    {
        if (!rows.TryGetValue(typeof(T), out var values))
            rows[typeof(T)] = values = new();
        return values;
    }

    private IQueryable<T> Query<T>() where T : class =>
        Rows<T>().Cast<T>().Select(Copy).AsQueryable();

    public void Insert<T>(T entity) where T : class
    {
        Rows<T>().Add(Copy(entity));
        InsertCalls++;
    }

    public void Update<T>(T entity) where T : class
    {
        var values = Rows<T>();
        var index = values.FindIndex(value => Equals(Id(value), Id(entity)));
        if (index < 0) throw new InvalidOperationException("Cannot update a missing fake entity.");
        values[index] = Copy(entity);
        UpdateCalls++;
    }

    public void RemoveUser(string id) => Rows<User>().RemoveAll(value => ((User)value).Id == id);

    private static object Id(object entity) => entity switch
    {
        User value => value.Id,
        Item value => value.Id,
        Category value => value.Id,
        Listing value => value.Id,
        Order value => value.Id,
        _ => throw new NotSupportedException()
    };

    private static T Copy<T>(T entity) where T : class => (T)(object)(entity switch
    {
        User value => new User { Id = value.Id, Username = value.Username, NormalizedUsername = value.NormalizedUsername, PasswordHash = value.PasswordHash },
        Item value => new Item { Id = value.Id, Name = value.Name },
        Category value => new Category { Id = value.Id, Name = value.Name },
        Listing value => new Listing { Id = value.Id, UserId = value.UserId, ItemId = value.ItemId, Price = value.Price, Quantity = value.Quantity, Description = value.Description },
        Order value => new Order { Id = value.Id, BuyerId = value.BuyerId, ListingId = value.ListingId, Quantity = value.Quantity, TotalPrice = value.TotalPrice, CreatedAt = value.CreatedAt },
        _ => throw new NotSupportedException()
    });
}
