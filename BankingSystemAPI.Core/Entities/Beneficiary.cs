namespace BankingSystemAPI.Core.Entities;

public class Beneficiary
{
    public int BeneficiaryId { get; set; }
    public int CustomerId { get; set; } //FK
    
    public Customer? Customer { get; set; }
}