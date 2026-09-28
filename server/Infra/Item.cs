using LinqToDB.Mapping;

namespace Infra;

public class Item
{
    [PrimaryKey]
    public int Id { get; set; }

    [Column]
    public string Name { get; set; } = "";
}