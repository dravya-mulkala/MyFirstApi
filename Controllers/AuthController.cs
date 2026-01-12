using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyFirstApi.Models;
using MyFirstApi.Services;

namespace MyFirstApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtTokenService tokenService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
    {
        if (req is null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest("Email and Password are required.");

        var user = FakeUserStore.Users.FirstOrDefault(
            u => u.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase));

        // BCrypt.Verify will work whether you call BCrypt.Net.BCrypt.Verify or just BCrypt.Verify if `using BCrypt.Net;`
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        // include role when creating the token
        var (token, expiresAt) = tokenService.CreateToken(user.Name, user.Email, user.Role);

        // include role in the response
        return new LoginResponse
        {
            Token = token,
            ExpiresAtUtc = expiresAt,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        // Try the standard .NET identity first, then fall back to JWT claim names
        var name = User.Identity?.Name
                   ?? User.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value
                   ?? "Unknown";

        var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                    ?? "unknown@example.com";

        var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value
                   ?? "Unknown";

        return Ok(new { name, email, role });
    }
}
