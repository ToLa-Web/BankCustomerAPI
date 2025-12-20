using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystemAPI.Core.Entities;

public class Transaction
{
    public int TransactionId { get; set; }
    public int AccountId { get; set; }
    // public int? ToAccountId { get; set; } // for transfer
    // public string TransactionType { get; set; } = string.Empty; // Deposit, Withdraw, Transfer
    // public decimal Amount { get; set; } = decimal.Zero;
    // public decimal BalanceAfter { get; set; } = decimal.Zero;
    // public string Description { get; set; } = string.Empty;
    //
    // public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    //
    public Account? Account { get; set; }
    public Account? ToAccount { get; set; } 
}