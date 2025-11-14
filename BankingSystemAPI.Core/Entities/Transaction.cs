using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystemAPI.Core.Entities;

public class Transaction
{
    [Key]
    public int TransactionId { get; set; }
    
    [Required]
    public int AccountId { get; set; }
    
    public int? ToAccountId { get; set; } // for transfer
    
    [Required, MaxLength(20)]
    public string TransactionType { get; set; } = string.Empty; // Deposit, Withdraw, Transfer
     
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Amount { get; set; } = decimal.Zero;
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal BalanceAfter { get; set; } = decimal.Zero;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    // Navigation properties
    [ForeignKey("AccountId")]
    public required Account Account { get; set; }
    
    [ForeignKey("ToAccountId")]
    public Account? ToAccount { get; set; } 
}