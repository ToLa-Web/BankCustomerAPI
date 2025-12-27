using BankingSystemAPI.Core.DTOs.Request.BeneficiaryRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Beneficiary;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface IBeneficiaryService
{
    Task<Result<BeneficiaryDto>> CreateAsync(int customerId, CreateBeneficiaryDto dto);
    Task<Result<IEnumerable<BeneficiaryDto>>> GetCustomerBeneficiariesAsync(int customerId);
    Task<Result<bool>> DeleteAsync(int customerId, int beneficiaryId);
    Task<Result<BeneficiaryDto>> SaveBeneficiaryFromTransferAsync(int customerId, string reference);
}