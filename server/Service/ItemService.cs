using Infra;

namespace Service;

public class ItemService(IApplicationData db)
{
    public List<Item> GetAll()
    {
        return db.Items
            .OrderBy(item => item.Id)
            .ToList();
    }
}
