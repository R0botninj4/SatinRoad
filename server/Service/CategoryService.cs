using System.ComponentModel.DataAnnotations;
using Infra;

namespace Service;

public class CategoryService(IApplicationData db)
{
    public List<Category> GetAll()
    {
        return db.Categories.ToList();
    }
    public Category? GetById(string id)
    {
        return db.Categories.FirstOrDefault(category => category.Id == id);
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
