using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Data.Context;

namespace BankingSystemAPI.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly BankingSystemDbContext _context;

    public UnitOfWork(BankingSystemDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> action)
    {
        // If there is already a transaction, reuse it
        if (_context.Database.CurrentTransaction != null)
        {
            await action();
            return;
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            await action();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw; // rethrow to preserve stack trace
        }
    }
}