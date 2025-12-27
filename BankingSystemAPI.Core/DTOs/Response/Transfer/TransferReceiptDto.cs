namespace BankingSystemAPI.Core.DTOs.Response.Transfer;

public class TransferReceiptDto
{
    public string Reference { get; set; } = string.Empty;

    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public DateTime TransferredAt { get; set; }
    public string Status { get; set; } = string.Empty;

    // Sender 
    public string SenderAccountNumber { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;

    // Receiver
    public string ReceiverAccountNumber { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public int ReceiverCustomerId {  get; set; }
}