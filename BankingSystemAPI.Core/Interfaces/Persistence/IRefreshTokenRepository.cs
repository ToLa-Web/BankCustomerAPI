using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByRefreshTokenAsync(string refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
}