using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<IEnumerable<User>> GetAllAsync();
    
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<bool> DeleteAsync(int userId);
} 