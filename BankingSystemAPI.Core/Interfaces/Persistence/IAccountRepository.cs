using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(int accountId);
    Task<IEnumerable<Account>> GetByCustomerAsync(int customerId);
    Task AddAsync(Account account);
    void Update(Account account);
    Task<Account?> GetByAccountNumberAsync(string accountNumber);
    Task<bool> ExistsAsync(int accountId);
    Task<int> GetAccountCountByCustomerAsync(int customerId);
    Task<IEnumerable<Account>> GetInactiveAccountsAsync();
    Task<IEnumerable<Account>> GetAllAsync(int? customerId, bool? isActive);
    Task<IEnumerable<Account>> GetAccountTypeAsync(int? customerId, AccountType? accountType);
    Task<IEnumerable<Account>> GetSavingsAccountsAsync();

    //Task<IEnumerable<Account>> GetAccountsMaturingBeforeAsync(DateTime date);
}