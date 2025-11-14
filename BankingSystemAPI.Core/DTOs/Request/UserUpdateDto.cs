using System.ComponentModel.DataAnnotations;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Request;

public class UserUpdateDto
{
    [MaxLength(50)]
    public string? Username { get; set; }
    [MaxLength(50)]
    public string? Email { get; set; }

    public UserRole? Role { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsEmailVerified { get; set; }

    public DateTime? LastLoginAt { get; set; }
}