using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Account;
using BankingSystemAPI.Core.DTOs.Response.AdminResponse;
using BankingSystemAPI.Core.DTOs.Response.Transfer;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Services.Mappings;
using BankingSystemAPI.Services.Services.Helper;
using BankingSystemAPI.Services.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BankingSystemAPI.Services.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly ITransactionRepository _transactionRepo;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository accountRepo,
        ICustomerRepository customerRepo, 
        ITransactionRepository transactionRepo,
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork,
        ILogger<AccountService> logger
        )
    {
        _accountRepo = accountRepo;
        _customerRepo = customerRepo;
        _transactionRepo = transactionRepo;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AccountDto>> CreateAccountAsync(int userId, CreateAccountDto dto, string? ip, string? device)
    {
        var customerExists = await _customerRepo.GetByUserIdAsync(userId);
        if (customerExists == null)
            return Result<AccountDto>.Fail("Customer not found");
        var validation = CreateAccountValidator.Validate(dto);
        if (!validation.Success)
            return Result<AccountDto>.Fail(validation.Message!);
        
        dto.Currency = dto.Currency.Trim().ToUpperInvariant();

        var account = new Account
        {
            CustomerId = customerExists.CustomerId,
            AccountNumber = GenerateAccountNumber(),
            AccountType = dto.AccountType,
            Currency = dto.Currency,
            Balance = 0,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        switch (dto.AccountType)
        {
            case AccountType.Savings:
                account.InterestRate = 0.015m;
                break;
            case AccountType.Current:
                account.InterestRate = 0m;
                break;
            case AccountType.FixedDeposit:
                account.InterestRate = 0.05m;
                account.MaturityDate = DateTime.UtcNow.AddMonths(dto.FixedTermMonths ?? 6);
                break;
        }

        await _accountRepo.AddAsync(account);
        await _auditLogService.LogAsync(userId, "Account created", ip, device);
        await _unitOfWork.SaveChangesAsync();

        return Result<AccountDto>.SuccessResult(AccountMapper.MapToAccountDto(account));
    }

    public async Task<Result<AccountDto>> DepositAsync(int accountId, decimal depositAmount, string? ip, string? device)
    {
        if (depositAmount <= 0)
            return Result<AccountDto>.Fail("Deposit amount must be greater than 0$");
        
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return Result<AccountDto>.Fail("Account not found");
        
        if (!account.IsActive)
            return Result<AccountDto>.Fail("Account is inactive");
        
        var balanceBefore = account.Balance;
        account.Balance += depositAmount;
        account.UpdatedAt = DateTime.UtcNow;
        
        var reference = Guid.NewGuid().ToString("N");

        var transaction = TransactionFactory.Create(
            account,
            TransactionType.Deposit,
            depositAmount,
            balanceBefore,
            account.Balance,
            reference,
            "Cash deposit");
        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _accountRepo.Update(account);
                await _transactionRepo.AddAsync(transaction);
                await _auditLogService.LogAsync(
                    account.CustomerId,
                    $"Account deposited {depositAmount}$",
                    ip,
                    device);
            });

            return Result<AccountDto>.SuccessResult(
                AccountMapper.MapToAccountDto(account),
                "Deposit successful");
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<AccountDto>.Fail(
                "Account was modified by another operation. Please try again.");
        }
    }

    public async Task<Result<AccountDto>> WithdrawAsync(int accountId, decimal withdrawAmount, string? ip, string? device)
    {
        if (withdrawAmount <= 0)
            return Result<AccountDto>.Fail("Invalid withdrawal amount");
        
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return Result<AccountDto>.Fail("Account not found");
        
        if (!account.IsActive)
            return Result<AccountDto>.Fail("Account is inactive");
        
        if (account.AccountType == AccountType.FixedDeposit && DateTime.UtcNow >  account.MaturityDate)
            return Result<AccountDto>.Fail("Cannot withdraw before maturity date");
        
        if (account.Balance < withdrawAmount)
            return Result<AccountDto>.Fail("Insufficient balance");
        
        var balanceBefore = account.Balance;
        account.Balance -= withdrawAmount;
        account.UpdatedAt = DateTime.UtcNow;
        
        var reference = Guid.NewGuid().ToString("N");

        var transaction = TransactionFactory.Create(
            account,
            TransactionType.Withdrawal,
            withdrawAmount,
            balanceBefore,
            account.Balance,
            reference,
            "Cash withdrawal");

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _accountRepo.Update(account);
                await _transactionRepo.AddAsync(transaction);
                await _auditLogService.LogAsync(
                    account.CustomerId,
                    $"Account withdrawal {withdrawAmount}$",
                    ip,
                    device);
            });

            return Result<AccountDto>.SuccessResult(
                AccountMapper.MapToAccountDto(account),
                "Withdrawal successful");
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<AccountDto>.Fail(
                "Account was modified by another operation. Please try again.");
        }
    }

    public async Task<Result<AccountDto?>> GetAccountByIdAsync(int userId, int accountId)
    {
        var customer = await _customerRepo.GetByIdAsync(userId);
        if (customer == null)
            return Result<AccountDto?>.Fail("Customer not found");
        
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return Result<AccountDto?>.Fail("Account not found");
        
        return account.CustomerId != customer.CustomerId ? Result<AccountDto?>.Fail("Access denied") : 
            Result<AccountDto?>.SuccessResult(AccountMapper.MapToAccountDto(account));
    }

    public async Task<Result<IEnumerable<AccountDto>>> GetAccountByCustomerAsync(int customerId)
    {
        var customerExists = await _customerRepo.GetByIdAsync(customerId);
        if (customerExists == null)
            return Result<IEnumerable<AccountDto>>.Fail("Customer not found");

        var accounts = await _accountRepo.GetByCustomerAsync(customerId);
        var accountsDto = accounts.Select(AccountMapper.MapToAccountDto).ToList();
        return accountsDto.Count == 0 ? Result<IEnumerable<AccountDto>>.Fail("Account not found") : 
            Result<IEnumerable<AccountDto>>.SuccessResult(accountsDto);
    }

    public async Task<Result<AccountBalanceDto>> GetAccountBalanceAsync(int userId, int accountId)
    {
        var customer = await _customerRepo.GetByIdAsync(userId);
        if (customer == null)
            return Result<AccountBalanceDto>.Fail("Customer not found");

        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return Result<AccountBalanceDto>.Fail("Account not found");

        if (account.CustomerId != customer.CustomerId)
            return Result<AccountBalanceDto>.Fail("Access denied");

        if (!account.IsActive)
            return Result<AccountBalanceDto>.Fail("Account is inactive");

        var dto = new AccountBalanceDto
        {
            AccountId = account.AccountId,
            AccountType = account.AccountType.ToString(),
            Balance = account.Balance,
            Currency = account.Currency
        };
        return Result<AccountBalanceDto>.SuccessResult(dto);
    }

    public async Task<Result<AccountDto>> GetAccountByAccountNumberAsync(string accountNumber)
    {
        var result = await _accountRepo.GetByAccountNumberAsync(accountNumber);
        return result == null ? Result<AccountDto>.Fail("Account not found") : 
            Result<AccountDto>.SuccessResult(AccountMapper.MapToAccountDto(result));
    }

    public async Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsAsync(int? customerId, bool? isActive)
    {
        var accountsResult = await _accountRepo.GetAllAsync(customerId, isActive);
        var accountsDto = accountsResult.Select(AccountMapper.MapToAccountAdminDto).ToList();
        return accountsDto.Count == 0 ? Result<IEnumerable<AccountAdminDto>>.Fail("Account not found") :
            Result<IEnumerable<AccountAdminDto>>.SuccessResult(accountsDto);
    }

    public async Task<Result<IEnumerable<AccountAdminDto>>> GetAccountsTypeAsync(int? customerId, AccountType? accountType)
    {
        var accountsResult = await _accountRepo.GetAccountTypeAsync(customerId, accountType);
        var accountsDto = accountsResult.Select(AccountMapper.MapToAccountAdminDto).ToList();
        return accountsDto.Count == 0 ? Result<IEnumerable<AccountAdminDto>>.Fail("Account not found") :
            Result<IEnumerable<AccountAdminDto>>.SuccessResult(accountsDto);
    }

    public async Task<Result<IEnumerable<AccountAdminDto>>> GetInactiveAccounts()
    {
        var accountsResult = await _accountRepo.GetInactiveAccountsAsync();
        var accountsDto = accountsResult.Select(AccountMapper.MapToAccountAdminDto).ToList();
        return accountsDto.Count == 0 ? Result<IEnumerable<AccountAdminDto>>.Fail("Account not found") :
            Result<IEnumerable<AccountAdminDto>>.SuccessResult(accountsDto);
    }

    public async Task<Result<PagedResult<TransactionDto>>> GetAccountTransactionsAsync(
        int userId,
        int accountId,
        int page,
        int pageSize)
    {
        if (page <= 0 ||  pageSize <= 0)
            return Result<PagedResult<TransactionDto>>.Fail("Invalid pagination parameters");
        var customer = await _customerRepo.GetByIdAsync(userId);
        if (customer == null)
            return Result<PagedResult<TransactionDto>>.Fail("Customer not found");
        
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null || account.CustomerId != customer.CustomerId)
            return Result<PagedResult<TransactionDto>>.Fail("Access denied");
        
        var pageTransactions = await _transactionRepo.GetByAccountIdAsync(accountId, page, pageSize);
        var dtoResult = new PagedResult<TransactionDto>
        {
            Page = pageTransactions.Page,
            PageSize = pageTransactions.PageSize,
            TotalCount = pageTransactions.TotalCount,
            Items = pageTransactions.Items.Select(t => new TransactionDto
            {
                TransactionId = t.TransactionId,
                TransactionType = t.TransactionType.ToString(),
                Amount = t.Amount,
                BalanceBefore = t.BalanceBefore,
                BalanceAfter = t.BalanceAfter,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
            })
        };
        
        return Result<PagedResult<TransactionDto>>.SuccessResult(dtoResult);
    }

    public async Task<Result<TransferResponseDto>> TransferAsync(int userId, TransferRequestDto dto, string? ip, string? device)
    {
        if (dto.Amount <= 0)
            return Result<TransferResponseDto>.Fail("Transfer amount must be greater than 0");
        if (dto.FromAccountId == dto.ToAccountId)
            return Result<TransferResponseDto>.Fail("Cannot transfer to the same account");
        var customer = await _customerRepo.GetByIdAsync(userId);
        if (customer == null)
            return Result<TransferResponseDto>.Fail("Customer not found");
        var sender = await _accountRepo.GetByIdAsync(dto.FromAccountId);
        var receiver = await _accountRepo.GetByIdAsync(dto.ToAccountId);
        
        if (sender == null || receiver == null)
            return Result<TransferResponseDto>.Fail("Account not found");
        
        if (sender.CustomerId != customer.CustomerId)
            return Result<TransferResponseDto>.Fail("Access denied");
        
        if (!sender.IsActive || !receiver.IsActive)
            return Result<TransferResponseDto>.Fail("One of the accounts is inactive");
        
        if (sender.Balance < dto.Amount)
            return Result<TransferResponseDto>.Fail("Insufficient balance");
        
        var senderBefore = sender.Balance;
        var receiverBefore = receiver.Balance;
        
        sender.Balance -= dto.Amount;
        receiver.Balance += dto.Amount;
        
        sender.UpdatedAt =  DateTime.UtcNow;
        receiver.UpdatedAt =  DateTime.UtcNow;
        
        var reference = Guid.NewGuid().ToString("N");

        var outTransaction = TransactionFactory.Create(
            sender,
            TransactionType.TransferOut,
            dto.Amount,
            senderBefore,
            sender.Balance,
            reference,
            dto.Description ?? $"Transfer to {receiver.AccountNumber}");
        outTransaction.RecipientAccountName = receiver.AccountNumber;

        var inTransaction = TransactionFactory.Create(
            receiver,
            TransactionType.TransferIn,
            dto.Amount,
            receiverBefore,
            receiver.Balance,
            reference,
            dto.Description ?? $"Transfer to {sender.AccountNumber}");
        inTransaction.RecipientAccountName = sender.AccountNumber;

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                _accountRepo.Update(sender);
                _accountRepo.Update(receiver);

                await _transactionRepo.AddAsync(outTransaction);
                await _transactionRepo.AddAsync(inTransaction);

                await _auditLogService.LogAsync(
                    customer.CustomerId,
                    $"Transferred {dto.Amount} from {sender.AccountNumber} to {receiver.AccountNumber}",
                    ip,
                    device);
            });

            var response = new TransferResponseDto
            {
                Reference = outTransaction.TransactionReference,
                Amount = dto.Amount,
                Currency = sender.Currency,
                FromAccount = sender.AccountNumber,
                ToAccount = receiver.AccountNumber,
                TransferredAt = outTransaction.CreatedAt,
            };

            return Result<TransferResponseDto>.SuccessResult(
                response,
                "Transfer success");
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<TransferResponseDto>.Fail(
                "One of the accounts was modified by another operation. Please try again.");
        }

    }

    public async Task<Result<TransferReceiptDto>> GetTransferByReferenceAsync(int userId, string reference)
    {
        var customer = await _customerRepo.GetByUserIdAsync(userId);
        if (customer == null)
            return Result<TransferReceiptDto>.Fail("Customer not found");
        
        var transactions = await _transactionRepo.GetByReferenceAsync(reference);
        if (!transactions.Any())
            return Result<TransferReceiptDto>.Fail("Transaction not found");
        // foreach (var tx in transactions)
        // {
        //     Console.WriteLine(
        //         $"TxId={tx.TransactionId}, Type={tx.TransactionType}, Ref={tx.TransactionReference}");
        // }
        // _logger.LogInformation(
        //     "Transactions for ref {Ref}: {@Transactions}",
        //     reference,
        //     transactions.Select(t => new { t.TransactionId, t.TransactionType })
        // );
        var outTx = transactions.FirstOrDefault(t => t.TransactionType == TransactionType.TransferOut);
        var inTx = transactions.FirstOrDefault(t => t.TransactionType == TransactionType.TransferIn);
        if (outTx == null || inTx == null)
            return Result<TransferReceiptDto>.Fail("Invalid transfer data");
        
        // Ownership check
        var senderAccount = await _accountRepo.GetByIdAsync(outTx.AccountId);
        if (senderAccount == null || senderAccount.CustomerId != customer.CustomerId)
            return Result<TransferReceiptDto>.Fail("Access denied");

        var receipt = new TransferReceiptDto
        {
            Reference = reference,
            Amount = outTx.Amount,
            Currency = senderAccount.Currency,
            FromAccount = senderAccount.AccountNumber,
            ToAccount = outTx.RecipientAccountName ?? "Unknown",
            TransferredAt = outTx.CreatedAt,
            Status = outTx.Status,
        };

        return Result<TransferReceiptDto>.SuccessResult(receipt);
    }

    public async Task<ResultDto> FreezeAccountAsync(int accountId, int adminUserId)
    {
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return ResultDto.Fail("Account not found");
        
        if (!account.IsActive)
            return ResultDto.Fail("Account is already frozen");

        account.IsActive = false;
        account.UpdatedAt = DateTime.UtcNow;
        
        _accountRepo.Update(account);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _auditLogService.LogAsync(
                adminUserId,
                $"Account {account.AccountNumber} frozen",
                null,
                "ADMIN");
        });

        return ResultDto.Ok("Account frozen successfully");
    }

    public async Task<ResultDto> UnfreezeAccountAsync(int accountId, int adminUserId)
    {
        var account = await _accountRepo.GetByIdAsync(accountId);
        if (account == null)
            return ResultDto.Fail("Account not found");
        
        if (account.IsActive)
            return ResultDto.Fail("Account is already active");

        account.IsActive = true;
        account.UpdatedAt = DateTime.UtcNow;
        
        _accountRepo.Update(account);

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _auditLogService.LogAsync(
                adminUserId,
                $"Account {account.AccountNumber} frozen",
                null,
                "ADMIN");
        });

        return ResultDto.Ok("Account unfrozen successfully");
    }

    private static string GenerateAccountNumber()
    {
        return "CUST-" + Guid.NewGuid().ToString()[..8].ToUpper();
    }
}