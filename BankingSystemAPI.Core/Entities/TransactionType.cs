namespace BankingSystemAPI.Core.Entities;

public enum TransactionType
{
    Deposit = 1,
    Withdrawal = 2,
    TransferIn = 3,
    TransferOut = 4,
    Fee = 5,
    Interest = 6
}