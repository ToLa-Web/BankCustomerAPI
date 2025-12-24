namespace BankingSystemAPI.Core.DTOs.Response.Transfer;

public class TransferReceiptDto
{
    public string Reference { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public string FromAccount { get; set; } = string.Empty;
    public string ToAccount { get; set; } = string.Empty;

    public DateTime TransferredAt { get; set; }

    public string Status { get; set; } = string.Empty;
}