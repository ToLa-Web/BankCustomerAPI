using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Account;
using BankingSystemAPI.Core.DTOs.Response.AdminResponse;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Services.Mappings;

public static class AccountMapper
{
    public static AccountDto MapToAccountDto(Account account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType.ToString(),
            Balance = account.Balance,
            Currency = account.Currency,
            CreatedAt = account.CreatedAt
        };
    }
    
    public static AccountAdminDto MapToAccountAdminDto(Account account)
    {
        return new AccountAdminDto
        {
            AccountId = account.AccountId,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType.ToString(),
            Balance = account.Balance,
            Currency = account.Currency,
            IsActive =  account.IsActive,   
            CreatedAt = account.CreatedAt
        };
    }
}