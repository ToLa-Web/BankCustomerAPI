using BankingSystemAPI.Core.DTOs.Response.AdminResponse;

namespace BankingSystemAPI.Core.Interfaces.Infrastructure;

public interface IInterestService
{
    Task<InterestBatchResult> ApplyMonthlyInterestAsync();
}