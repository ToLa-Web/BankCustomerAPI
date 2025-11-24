using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Services;

public interface IEmailVerificationTokenService
{
    Task<EmailVerificationToken> GenerateAndSaveTokenAsync(int userId, TokenType tokenType, TimeSpan expiry);
    Task<EmailVerificationToken?> ValidateTokenAsync(string token, TokenType tokenType);
    Task MarkTokenUsedAsync(EmailVerificationToken token);
}