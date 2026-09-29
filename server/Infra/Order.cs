using LinqToDB.Mapping;

namespace Infra;

public class Order
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    [Column]
    public string BuyerId { get; set; } = "";

    [Column]
    public string ListingId { get; set; } = "";

    [Column]
    public int Quantity { get; set; }

    [Column]
    public decimal TotalPrice { get; set; }

    [Column]
    public DateTime CreatedAt { get; set; }
}