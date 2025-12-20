using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Persistence;

namespace BankingSystemAPI.Services.Services;

public class EmailVerificationTokenService : IEmailVerificationTokenService
{
    private readonly IEmailVerificationTokenRepository _tokenRepo;

    public EmailVerificationTokenService(IEmailVerificationTokenRepository tokenRepo)
    {
        _tokenRepo = tokenRepo;
    }
    
    //create token & store in DB for register
    public async Task<EmailVerificationToken> GenerateAndSaveTokenAsync(int userId, TokenType tokenType, TimeSpan expiry)
    {
        var token = new EmailVerificationToken
        {
            UserId = userId,
            Token = Guid.NewGuid().ToString(),
            TokenType = tokenType,
            ExpiryDate = DateTime.UtcNow.Add(expiry),
            UsedAt = null
        };

        await _tokenRepo.AddAsync(token);
        
        return token;
    }
    
    //check token validity & expiration
    public async Task<EmailVerificationToken?> ValidateTokenAsync(string token, TokenType tokenType)
    {
        return await _tokenRepo.GetValidTokenAsync(token, tokenType);
    }
    
    //update UsedAt after verification
    public Task MarkTokenUsedAsync(EmailVerificationToken token)
    {
        token.UsedAt = DateTime.UtcNow;
        return _tokenRepo.UpdateAsync(token);
    }
}