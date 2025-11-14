using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Core.Models;

public class Customer
{
    [Key]
    public int CustomerId { get; set; } //PK

    [Required, MaxLength(100)] 
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(100)] 
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Account> Accounts { get; set; } =  new List<Account>(); // One to Many
    
    // IF it one to one it would be like this public Account Account { get; set; } 
}