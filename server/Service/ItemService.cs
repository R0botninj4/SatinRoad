using Infra;

namespace Service;

public class ItemService(MyDatabaseConnection db)
{
    public List<Item> GetAll()
    {
        return db.Items
            .OrderBy(item => item.Id)
            .ToList();
    }
}