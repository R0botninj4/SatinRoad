using System.ComponentModel.DataAnnotations;
using Infra;
using LinqToDB;

namespace Service;

public class CategoryService(MyDatabaseConnection db)
{
    public List<Category> GetAll()
    {
        return db.Categories.ToList();
    }

    public Category Create(CreateCategoryRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException("Category name is required.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name.Trim()
        };

        db.Insert(category);

        return category;
    }
}