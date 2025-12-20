using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly BankingSystemDbContext _context;
    public EmailVerificationTokenRepository(BankingSystemDbContext context)
    {
        _context = context;
    }

    // Save token
    public async Task AddAsync(EmailVerificationToken token)
    {
        await _context.EmailVerificationTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    // Validate token 
    public async Task<EmailVerificationToken?> GetValidTokenAsync(string token, TokenType tokenType)
    {
        return await _context.EmailVerificationTokens
            .FirstOrDefaultAsync(t 
                => t.Token == token &&
                   t.TokenType == tokenType &&
                   t.UsedAt == null &&
                   t.ExpiryDate > DateTime.UtcNow
                   );
    }
    
    // Update token as used
    public async Task UpdateAsync(EmailVerificationToken token)
    {
        _context.EmailVerificationTokens.Update(token);
        await _context.SaveChangesAsync();
    }

}