using System.ComponentModel.DataAnnotations;

namespace Service.Tests;

public class CategoryServiceTests : TestDatabase
{
    [Fact]
    public void Category_is_trimmed_persisted_and_retrievable()
    {
        var service = new CategoryService(Db);
        var category = service.Create(new() { Name = "  Materials  " });
        Assert.Equal("Materials", category.Name);
        Assert.Equal(category.Id, Assert.Single(service.GetAll()).Id);
        Assert.Equal("Materials", service.GetById(category.Id)?.Name);
        Assert.Null(service.GetById("missing"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t\n")]
    public void Empty_category_name_is_rejected_without_inserting(string name)
    {
        var service = new CategoryService(Db);
        Assert.Throws<ValidationException>(() => service.Create(new() { Name = name }));
        Assert.Empty(service.GetAll());
    }
}
