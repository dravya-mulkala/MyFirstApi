namespace MyFirstApi.Models;

public record LoginRequest(string Email, string Password);

public class LoginResponse
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAtUtc { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role{ get; set; } = default!;
}
