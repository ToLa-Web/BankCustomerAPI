namespace BankingSystemAPI.Core.DTOs.Response.Beneficiary;

public class BeneficiaryDto
{
    public int BeneficiaryId { get; set; }
    public string BeneficiaryName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankCode { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public bool IsOurBank { get; set; }
    public string? Nickname { get; set; }
    public DateTime CreatedAt { get; set; }
}