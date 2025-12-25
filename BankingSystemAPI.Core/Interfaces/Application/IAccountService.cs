using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Account;
using BankingSystemAPI.Core.DTOs.Response.AdminResponse;
using BankingSystemAPI.Core.DTOs.Response.Transfer;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface IAccountService
{
    Task<Result<AccountDto>> CreateAccountAsync(int userId, CreateAccountDto dto, string? ip, string? device);
    Task<Result<AccountDto>> DepositAsync(int accountId, decimal depositAmount, string? ip, string? device);
    Task<Result<AccountDto>> WithdrawAsync(int accountId, decimal withdrawAmount, string? ip, string? device);
    Task<Result<AccountDto?>> GetAccountByIdAsync(int performedByUserId, int accountId);
    Task<Result<IEnumerable<AccountDto>>> GetAccountByCustomerAsync(int customerId);
    Task<Result<AccountBalanceDto>> GetAccountBalanceAsync(int userId, int accountId);
    Task<Result<AccountDto>> GetAccountByAccountNumberAsync(string accountNumber);
    Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsAsync(int? customerId, bool? isActive);
    Task<Result<IEnumerable<AccountAdminDto>>> GetInactiveAccounts();
    Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsTypeAsync(int? customerId, AccountType? accountType);
    Task<Result<PagedResult<TransactionDto>>> GetAccountTransactionsAsync(int userId, int accountId, int page, int pageSize);
    Task<Result<TransferResponseDto>> TransferAsync(int userId, TransferRequestDto dto, string? ip, string? device);
    Task<Result<TransferReceiptDto>> GetTransferByReferenceAsync(int userId, string reference);
    Task<ResultDto> FreezeAccountAsync(int accountId, int adminUserId);
    Task<ResultDto> UnfreezeAccountAsync(int accountId, int adminUserId);
    
}

