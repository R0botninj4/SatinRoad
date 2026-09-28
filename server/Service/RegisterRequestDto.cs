using System.ComponentModel.DataAnnotations;

namespace Service;

public class RegisterRequestDto
{
    [Required, StringLength(30, MinimumLength = 3)]
    [RegularExpression(@"[a-zA-Z0-9_]+", ErrorMessage = "Use letters, numbers or underscores.")]
    public string Username { get; set; } = "";

    [Required, StringLength(128, MinimumLength = 12)]
    public string Password { get; set; } = "";
}
