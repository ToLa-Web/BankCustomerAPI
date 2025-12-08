using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class Customer
{
    public int CustomerId { get; set; } //PK
    public int UserId { get; set; } //FK
    
    public string NationalId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth {get; set;}
    public string? Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string CustomerNumber { get; set; } = string.Empty;
    
    public CustomerVerificationStatus VerificationStatus { get; set; } =  CustomerVerificationStatus.None;
    public CustomerStatus Status { get; set; } = CustomerStatus.Active; // Active / Suspended / Closed
    
    public int? VerifiedByUserId { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    // Navigation properties
    public User? User { get; set; } // 1:1
    public User? VerifiedByUser { get; set; }
    public ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>(); // 1:N
    public ICollection<Account> Accounts { get; set; } =  new List<Account>(); // 1:N
    
}