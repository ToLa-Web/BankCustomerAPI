using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface IBeneficiariesRepo
{
    Task AddAsync(Beneficiary beneficiary);
    void Remove(Beneficiary beneficiary);
    Task<Beneficiary?> GetByIdAsync(int id);
    Task<IEnumerable<Beneficiary>> GetByCustomerAsync(int customerId);
    Task<bool> ExistsAsync(int customerId, string accountNumber, string bankCode);
}