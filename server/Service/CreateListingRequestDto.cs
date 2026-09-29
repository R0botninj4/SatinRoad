using System.ComponentModel.DataAnnotations;

namespace Service;

public class CreateListingRequestDto
{
    [Range(1, int.MaxValue)]
    public int ItemId { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public string Description { get; set; } = "";
}
