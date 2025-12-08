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
}