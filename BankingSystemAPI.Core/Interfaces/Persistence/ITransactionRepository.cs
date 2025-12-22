using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction);
    Task<List<Transaction>> GetByAccountIdAsync(int accountId);
    Task<PagedResult<Transaction>> GetByAccountIdAsync(
        int accountId,
        int page,
        int pageSize);
}