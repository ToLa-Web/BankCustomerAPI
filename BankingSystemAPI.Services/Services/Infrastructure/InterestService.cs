using BankingSystemAPI.Core.DTOs.Response.AdminResponse;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Infrastructure;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Services.Services.Helper;

namespace BankingSystemAPI.Services.Services.Infrastructure;

public class InterestService : IInterestService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IUnitOfWork _unitOfWork;

    public InterestService(IAccountRepository accountRepository,  ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
    {
        _accountRepo = accountRepository;
        _transactionRepo = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<InterestBatchResult> ApplyMonthlyInterestAsync()
    {
        var result = new InterestBatchResult();
        var accounts = await _accountRepo.GetSavingsAccountsAsync();

        foreach (var account in accounts)
        {
            var interestAmount = CalculateMonthlyInterest(account.Balance, account.InterestRate);
            
            if (interestAmount <= 0)
                continue;
            
            var balanceBefore = account.Balance;
            account.Balance += interestAmount;
            account.UpdatedAt = DateTime.Now;
            
            var reference = result.BatchReference;

            var transaction = TransactionFactory.Create(
                account,
                TransactionType.Interest,
                interestAmount,
                balanceBefore,
                account.Balance,
                reference,
                "Monthly interest"
            );

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _accountRepo.Update(account);
                await _transactionRepo.AddAsync(transaction);
            });
            result.AccountsProcessed++;
            result.TotalInterestPaid += interestAmount;
        }
        return result;
    }
    
    private static decimal CalculateMonthlyInterest(decimal balance, decimal rate)
    {
        return Math.Round(balance * (rate / 12), 2);
    }
}