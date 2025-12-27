namespace BankingSystemAPI.Core.DTOs.Request.BeneficiaryRequest;

public class CreateBeneficiaryDto
{
    public string BeneficiaryName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string BankCode { get; set; } = string.Empty;
    public string? Nickname { get; set; }
}