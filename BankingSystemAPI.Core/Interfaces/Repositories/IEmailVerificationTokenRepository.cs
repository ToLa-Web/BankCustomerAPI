using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository
{
    Task AddAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetValidTokenAsync(string token, TokenType tokenType);
    Task UpdateAsync(EmailVerificationToken token);
}