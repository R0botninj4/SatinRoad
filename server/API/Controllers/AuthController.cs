using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

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
    [ProducesResponseType(typeof(AccessTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login(LoginRequestDto dto)
    {
        var user = userService.Login(dto);
        if (user is null) return Unauthorized(new { message = "Invalid username or password." });

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Username)
        }, BearerTokenDefaults.AuthenticationScheme);

        return SignIn(new ClaimsPrincipal(identity), BearerTokenDefaults.AuthenticationScheme);
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UserResponseDto> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = id is null ? null : userService.GetById(id);
        if (user is null) return Unauthorized();
        return Ok(user);
    }
}
