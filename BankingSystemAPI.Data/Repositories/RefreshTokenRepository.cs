using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BankingSystemDbContext _context;

    public RefreshTokenRepository(BankingSystemDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync();
    }
}

