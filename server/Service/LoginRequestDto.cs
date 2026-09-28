using System.ComponentModel.DataAnnotations;

namespace Service;

public class LoginRequestDto
{
    [Required, StringLength(30)]
    public string Username { get; set; } = "";

    [Required, StringLength(128)]
    public string Password { get; set; } = "";
}
