using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class User
{
    public int UserId { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public UserRole  Role { get; set; } = UserRole.Customer;
    public bool IsEmailVerified { get; set; }
    public string? VerificationToken { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false; // Soft delete
    public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
    
    public DateTime? LastLoginAt { get; set; }
    [NotMapped]
    public Customer? Customer { get; set; }
}