using BankingSystemAPI.Core.DTOs.Response.Beneficiary;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Services.Mappings;

public static class BeneficiaryMapper
{
    public static BeneficiaryDto MapToBeneficiaryDto(Beneficiary beneficiary, string ourBankCode)
    {
        return new BeneficiaryDto
        {
            BeneficiaryId = beneficiary.BeneficiaryId,
            BeneficiaryName = beneficiary.BeneficiaryName,
            AccountNumber = beneficiary.AccountNumber,
            BankCode = beneficiary.BankCode,
            BankName = beneficiary.BankName,
            IsOurBank = beneficiary.BankCode.Equals(ourBankCode, StringComparison.OrdinalIgnoreCase),
            Nickname = beneficiary.Nickname,
            CreatedAt = beneficiary.CreatedAt
        };
    }
}