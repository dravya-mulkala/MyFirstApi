// File: Services/FakeUserStore.cs
using BCrypt.Net;

namespace MyFirstApi.Services;

// Simple in-memory user record for demo/testing
public record FakeUser(string Name, string Email, string PasswordHash, string Role);

public static class FakeUserStore
{
    // For demo: password for both users is "Pass@123"
    public static readonly List<FakeUser> Users =
    [
        new FakeUser(
            Name: "Demo Admin",
            Email: "admin@example.com",
            PasswordHash: BCrypt.Net.BCrypt.HashPassword("Pass@123"),
            Role: "Admin"
        ),
        new FakeUser(
            Name: "Demo User",
            Email: "user@example.com",
            PasswordHash: BCrypt.Net.BCrypt.HashPassword("Pass@123"),
            Role: "User"
        )
    ];
}
