using LinqToDB.Mapping;

namespace Infra;

public class Listing
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    [Column]
    public string UserId { get; set; } = "";

    [Column]
    public int ItemId { get; set; }

    [Column]
    public decimal Price { get; set; }

    [Column]
    public int Quantity { get; set; }

    [Column]
    public string Description { get; set; } = "";
}