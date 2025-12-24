using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly BankingSystemDbContext _context;

    public TransactionRepository(BankingSystemDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transaction transaction)
    {
        await _context.Transactions.AddAsync(transaction);
    }

    public async Task<List<Transaction>> GetByAccountIdAsync(int accountId)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<PagedResult<Transaction>> GetByAccountIdAsync(
        int accountId,
        int page,
        int pageSize)
    {
        var query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId);
        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Transaction>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IReadOnlyList<Transaction>> GetByReferenceAsync(string reference)
    {
        return await _context.Transactions
            .AsNoTracking()
            .Where(t => t.TransactionReference == reference)
            .ToListAsync();
    }
}