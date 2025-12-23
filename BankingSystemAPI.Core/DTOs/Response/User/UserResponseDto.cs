using System.Text.Json.Serialization;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Response.User;

public class UserResponseDto
{
    public int UserId { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UserRole  Role { get; set; }
    public bool IsEmailVerified {get; set;}
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt  { get; set; }
}