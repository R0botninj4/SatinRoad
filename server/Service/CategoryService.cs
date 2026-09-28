using Infra;

namespace Service;

public class CategoryService(MyDatabaseConnection db)
{
    public List<Category> GetAll()
    {
        return db.Categories.ToList();
    }
}