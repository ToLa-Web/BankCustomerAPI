namespace BankingSystemAPI.Core.DTOs.Response.Account;

public class AccountDto
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = null!;
    public string AccountType { get; set; } = null!;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}