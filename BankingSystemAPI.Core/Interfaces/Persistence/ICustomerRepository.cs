using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface ICustomerRepository
{
    Task<Customer?> GetByUserIdAsync(int userId);
    Task<Customer?> GetByIdAsync(int customerId);
    Task<IEnumerable<Customer>> GetAllAsync();
    
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    
    Task<bool> NationalExistAsync(string nationalId, int? excludeCustomerId = null);
    Task<bool> PhoneExistAsync(string phoneNumber, int? excludeCustomerId = null);
    
    Task<List<Customer>> GetPendingCustomersAsync();
}