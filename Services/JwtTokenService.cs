using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MyFirstApi.Services
{
    public class JwtOptions
    {
        public string Key { get; set; } = "";
        public string Issuer { get; set; } = "";
        public string Audience { get; set; } = "";
        public int ExpiryMinutes { get; set; } = 60;
    }

    public interface IJwtTokenService
    {
        (string token, DateTime expiresAtUtc) CreateToken(string name, string email, string role);
    }

    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtOptions _opt;

        public JwtTokenService(IOptions<JwtOptions> options)
        {
            _opt = options.Value ?? throw new ArgumentNullException(nameof(options));

            // Basic sanity checks to catch misconfigurations early
            if (string.IsNullOrWhiteSpace(_opt.Key))
                throw new InvalidOperationException("JwtOptions.Key must be configured.");
            if (string.IsNullOrWhiteSpace(_opt.Issuer))
                throw new InvalidOperationException("JwtOptions.Issuer must be configured.");
            if (string.IsNullOrWhiteSpace(_opt.Audience))
                throw new InvalidOperationException("JwtOptions.Audience must be configured.");
            if (_opt.ExpiryMinutes <= 0)
                throw new InvalidOperationException("JwtOptions.ExpiryMinutes must be greater than zero.");
        }

        public (string token, DateTime expiresAtUtc) CreateToken(string name, string email, string role)
        {
            // Build claims
            var claims = new List<Claim>
            {
                // Standard JWT claims
                new(JwtRegisteredClaimNames.Sub, email),                // subject
                new(JwtRegisteredClaimNames.Email, email),              // email
                new(JwtRegisteredClaimNames.UniqueName, name),          // unique_name
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                // .NET-friendly identity claims
                new(ClaimTypes.Name, name),                             // so User.Identity.Name works
                new(ClaimTypes.NameIdentifier, email),                  // stable id (using email for demo)
                new(ClaimTypes.Role, role)                              // role-based auth
            };

            // Sign token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_opt.ExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return (jwt, expires);
        }
    }
}
