using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Response.Account;

public class AccountAdminDto
{
    public int AccountId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}