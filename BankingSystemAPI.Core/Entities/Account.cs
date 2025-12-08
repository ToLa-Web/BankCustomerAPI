using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystemAPI.Core.Entities;

public class Account
{
    [Key]
    public int AccountId { get; set; } //PK
    
    // public string AccountNumber { get; set; } =  string.Empty;
    //
    // public string AccountType { get; set; } =  string.Empty; //saving, checking etc...
    //
    // public decimal Balance { get; set; } = decimal.Zero;
    //
    public int CustomerId { get; set; } //FK 
    //
    // public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //
    // public bool IsActive { get; set; } = true;
    //
    // //Navigation property
    public Customer? Customer { get; set; } 
    public ICollection<Transaction> Transactions { get; set; } =  new List<Transaction>(); // One to Many
}