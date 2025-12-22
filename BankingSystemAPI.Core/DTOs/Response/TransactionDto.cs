using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.DTOs.Response;

public class TransactionDto
{
    public int TransactionId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal BalanceBefore { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}