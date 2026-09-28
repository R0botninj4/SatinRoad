using System.ComponentModel.DataAnnotations;

namespace Service;

public class CreateCategoryRequestDto
{
    [Required]
    public string Name { get; set; } = "";
}