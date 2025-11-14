using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankingSystemAPI.Core.Models;

public class Account
{
    [Key]
    public int AccountId { get; set; } //PK
    
    [Required, MaxLength(20)]
    public string AccountNumber { get; set; } =  string.Empty;
    
    [Required, MaxLength(50)]
    public string AccountType { get; set; } =  string.Empty; //saving, checking etc...
    
    [Column(TypeName = "decimal(18, 2")]
    public decimal Balance { get; set; } = decimal.Zero;
    
    [Required]
    public int CustomerId { get; set; } //FK 
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    //Navigation property
    [ForeignKey("CustomerId")]
    public required Customer Customer { get; set; } 
    
    public ICollection<Transaction> Transactions { get; set; } =  new List<Transaction>(); // One to Many
}