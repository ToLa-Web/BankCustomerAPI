using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Account;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Services.Mappings;
using BankingSystemAPI.Services.Validators;

namespace BankingSystemAPI.Services.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(
        IAccountRepository accountRepo,
        ICustomerRepository customerRepo, 
        IAuditLogService auditLogService,
        IUnitOfWork unitOfWork
        )
    {
        _accountRepo = accountRepo;
        _customerRepo = customerRepo;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AccountDto>> CreateAccountAsync(int customerId, CreateAccountDto dto, string? ip, string? device)
    {
        var customerExists = await _customerRepo.GetByIdAsync(customerId);
        if (customerExists == null)
            return Result<AccountDto>.Fail("Customer not found");
        var validation = CreateAccountValidator.Validate(dto);
        if (!validation.Success)
            return Result<AccountDto>.Fail(validation.Message!);
        
        dto.Currency = dto.Currency.Trim().ToUpperInvariant();

        var account = new Account
        {
            CustomerId = customerId,
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
                account.InterestRate = 1.5m;
                break;
            case AccountType.Current:
                account.InterestRate = 0m;
                break;
            case AccountType.FixedDeposit:
                account.InterestRate = 5m;
                account.MaturityDate = DateTime.UtcNow.AddMonths(dto.FixedTermMonths ?? 6);
                break;
        }

        await _accountRepo.AddAsync(account);
        await _auditLogService.LogAsync(customerId, "Account created", ip, device);
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
        
        account.Balance += depositAmount;
        account.UpdatedAt = DateTime.UtcNow;
        
        _accountRepo.Update(account);
        await _auditLogService.LogAsync(account.CustomerId, $"Account deposited {depositAmount}$", ip, device);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<AccountDto>.SuccessResult(AccountMapper.MapToAccountDto(account), 
            "Deposit successful");
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
        
        account.Balance -= withdrawAmount;
        account.UpdatedAt = DateTime.UtcNow;
        
        _accountRepo.Update(account);
        await _auditLogService.LogAsync(account.CustomerId, $"Account withdrawal {withdrawAmount}$ successful", ip, device);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<AccountDto>.SuccessResult(AccountMapper.MapToAccountDto(account),
            "Withdrawal successful");
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

    private static string GenerateAccountNumber()
    {
        return "CUST-" + Guid.NewGuid().ToString()[..8].ToUpper();
    }
}