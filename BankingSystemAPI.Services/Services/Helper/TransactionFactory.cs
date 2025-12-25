using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Services.Services.Helper;

public static class TransactionFactory
{
    public static Transaction Create(
        Account account,
        TransactionType type,
        decimal amount,
        decimal balanceBefore,
        decimal balanceAfter,
        string reference,
        string? description = null)
    {
        return new Transaction
        {
            AccountId = account.AccountId,
            TransactionType = type,
            Amount = amount,
            BalanceBefore = balanceBefore,
            BalanceAfter = balanceAfter,
            TransactionReference = reference,
            Description = description
        };
    }
}