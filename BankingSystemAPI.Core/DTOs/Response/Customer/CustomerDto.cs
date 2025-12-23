namespace BankingSystemAPI.Core.DTOs.Response.Customer;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerNumber { get; set; } = string.Empty;

    public string NationalId { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int VerificationStatus { get; set; }
    public string VerificationStatusName { get; set; } = string.Empty;
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
}