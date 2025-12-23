using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Entities;

public class Account
{
    public int AccountId { get; set; } //PK
    public int CustomerId { get; set; } 
    
    public string AccountNumber { get; set; } =  string.Empty;
    public AccountType AccountType { get; set; } //saving, checking etc...
    public decimal Balance { get; set; } = decimal.Zero;
    
    public string Currency { get; set; } = "USD";
    
    public decimal InterestRate { get; set; }
    public DateTime? MaturityDate { get; set; } // Only used for FixedDeposit
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Customer? Customer { get; set; } 
    public ICollection<Transaction> Transactions { get; set; } =  new List<Transaction>();
}