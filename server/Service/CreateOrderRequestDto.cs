using System.ComponentModel.DataAnnotations;

namespace Service;

public class CreateOrderRequestDto
{
    [Required]
    public string ListingId { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}