using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int AccountId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public TransactionType TransactionType { get; set; }
    
    public decimal Amount { get; set; } = decimal.Zero;
    public decimal BalanceBefore { get; set; } = decimal.Zero;
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Completed";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Account? Account { get; set; }
    // Transfer-only fields
    public string? RecipientAccount { get; set; }
    public string? RecipientAccountName { get; set; }
}