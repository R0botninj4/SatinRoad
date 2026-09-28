using LinqToDB.Mapping;

namespace Infra;

public class Category
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    [Column]
    public string Name { get; set; } = "";
}