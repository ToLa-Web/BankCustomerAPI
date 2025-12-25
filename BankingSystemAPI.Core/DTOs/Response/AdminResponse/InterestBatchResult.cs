namespace BankingSystemAPI.Core.DTOs.Response.AdminResponse;

public class InterestBatchResult
{
    public int AccountsProcessed { get; set; }
    public decimal TotalInterestPaid { get; set; }
    public string BatchReference { get; set; } = Guid.NewGuid().ToString("N");
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}