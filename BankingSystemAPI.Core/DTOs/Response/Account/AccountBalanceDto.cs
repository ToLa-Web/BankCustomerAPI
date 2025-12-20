using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Response.Account;

public class AccountBalanceDto
{
    public int AccountId { get; set; } 
    public string? AccountType { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = null!;
}