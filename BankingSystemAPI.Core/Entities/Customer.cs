using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class Customer
{
    [Key]
    public int CustomerId { get; set; } //PK
    public int UserId { get; set; } //FK
    public User User { get; set; } = null!; 
    
    [Required, MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public DateTime DateOfBirth {get; set;}
    
    [Required, MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    public bool IsEmailVerified { get; set; }

    [Required, MaxLength(20)]
    public CustomerStatus Status { get; set; } = CustomerStatus.Active; // Active / Suspended / Closed
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
    public ICollection<Account> Accounts { get; set; } =  new List<Account>(); // One to Many
    
    // IF it one to one it would be like this public Account Account { get; set; } 
}