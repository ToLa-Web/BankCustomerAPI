using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Account;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface IAccountService
{
    Task<Result<AccountDto>> CreateAccountAsync(int customerId, CreateAccountDto dto, string? ip, string? device);
    Task<Result<AccountDto>> DepositAsync(int accountId, decimal depositAmount, string? ip, string? device);
    Task<Result<AccountDto>> WithdrawAsync(int accountId, decimal withdrawAmount, string? ip, string? device);
    Task<Result<AccountDto?>> GetAccountByIdAsync(int performedByUserId, int accountId);
    Task<Result<IEnumerable<AccountDto>>> GetAccountByCustomerAsync(int customerId);
    Task<Result<AccountBalanceDto>> GetAccountBalanceAsync(int userId, int accountId);
    Task<Result<AccountDto>> GetAccountByAccountNumberAsync(string accountNumber);
    Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsAsync(int? customerId, bool? isActive);
    Task<Result<IEnumerable<AccountAdminDto>>> GetInactiveAccounts();
    Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsTypeAsync(int? customerId, AccountType? accountType);
}

