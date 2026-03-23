namespace HouseholdInventory.Application.DTOs;

public record LoginRequest(string Email, string Password);
public record RegisterUserRequest(string Email, string Password, string FullName, string Role);
public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Email, string Role, string FullName);
