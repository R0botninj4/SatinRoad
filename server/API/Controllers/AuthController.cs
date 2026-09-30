using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Service;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(UserService userService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<UserResponseDto> Register(RegisterRequestDto dto)
    {
        var user = userService.Register(dto);
        if (user is null) return Conflict(new { message = "Username is already taken." });
        return StatusCode(StatusCodes.Status201Created, user);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponseDto>> Login(LoginRequestDto dto)
    {
        var user = userService.Login(dto);
        if (user is null) return Unauthorized(new { message = "Invalid username or password." });

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return Ok(user);
    }
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponseDto> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            return Unauthorized();
        }

        var user = userService.GetById(userId);

        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(user);
    }
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return NoContent();
    }
}
