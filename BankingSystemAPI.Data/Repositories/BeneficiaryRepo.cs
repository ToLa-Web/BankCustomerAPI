using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class BeneficiaryRepo : IBeneficiariesRepo
{
    private readonly BankingSystemDbContext _context;

    public BeneficiaryRepo(BankingSystemDbContext context)
    {
        _context = context;
    }
    public Task<Beneficiary?> GetByIdAsync(int id)
        => _context.Beneficiaries.FirstOrDefaultAsync(b => b.BeneficiaryId == id);

    public async Task<IEnumerable<Beneficiary>> GetByCustomerAsync(int customerId)
        => await _context.Beneficiaries
            .Where(b => b.CustomerId == customerId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

    public Task<bool> ExistsAsync(int customerId, string accountNumber, string bankCode)
        => _context.Beneficiaries
            .AnyAsync(b => b.CustomerId == customerId &&
                           b.AccountNumber == accountNumber &&
                           b.BankCode == bankCode);

    public Task AddAsync(Beneficiary beneficiary)
        => _context.Beneficiaries.AddAsync(beneficiary).AsTask();

    public void Remove(Beneficiary beneficiary)
        => _context.Beneficiaries.Remove(beneficiary);
}