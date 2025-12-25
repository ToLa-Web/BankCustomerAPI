using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly BankingSystemDbContext _context;
    public AccountRepository(BankingSystemDbContext context)
    {
        _context = context;
    }
    public async Task<Account?> GetByIdAsync(int accountId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountId == accountId);
    }

    public async Task<IEnumerable<Account>> GetByCustomerAsync(int customerId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _context.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
    }

    public async Task<bool> ExistsAsync(int accountId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.AccountId == accountId);
    }

    public async Task<int> GetAccountCountByCustomerAsync(int customerId)
    {
        return await _context.Accounts
            .AsNoTracking()
            .CountAsync(a => a.CustomerId == customerId);
    }

    public async Task<IEnumerable<Account>> GetInactiveAccountsAsync()
    {
        return await _context.Accounts.AsNoTracking()
            .Where(a => a.IsActive == false)
            .ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetAllAsync(int? customerId, bool? isActive)
    {
        IQueryable<Account> query = _context.Accounts.AsNoTracking();

        if (customerId.HasValue) 
            query = query.Where(a => a.CustomerId == customerId.Value);
        
        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);
        
        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetAccountTypeAsync(int? customerId, AccountType? accountType)
    {
        var query = _context.Accounts.AsNoTracking();

        if (customerId.HasValue)
            query = query.Where(a => a.CustomerId == customerId.Value);

        if (accountType.HasValue)
            query = query.Where(a => a.AccountType == accountType.Value);
        
        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetSavingsAccountsAsync()
    {
        return await _context.Accounts
            .Where(a =>
                a.AccountType == AccountType.Savings &&
                a.IsActive &&
                a.Balance > 0)
            .ToListAsync();
    }
}