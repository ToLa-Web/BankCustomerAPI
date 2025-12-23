using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Response.Auth;

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsEmailVerified { get; set; }
    public string Token { get; set; } = string.Empty; // Access token
    public string RefreshToken { get; set; } = string.Empty; // New Refresh token
    public DateTime? LastLoginAt { get; set; } 
}