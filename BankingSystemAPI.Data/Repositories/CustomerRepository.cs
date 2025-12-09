using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly BankingSystemDbContext _context;
    public CustomerRepository(BankingSystemDbContext context)
    {
        _context = context;
    }
    public async Task<Customer?> GetByUserIdAsync(int userId)
    {
        return await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Customer?> GetByIdAsync(int customerId)
    {
        return await _context.Customers.AsNoTracking().Include(c => c.User).FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }
    
    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        return await _context.Customers.ToListAsync();
    }

    public async Task<bool> NationalExistAsync(string nationalId, int? excludeCustomerId = null)
    {
        return await _context.Customers.AnyAsync(c => 
            c.NationalId == nationalId && 
            (!excludeCustomerId.HasValue || c.CustomerId != excludeCustomerId.Value));
    }

    public async Task<bool> PhoneExistAsync(string phoneNumber, int? excludeCustomerId = null)
    {
        return await _context.Customers.AnyAsync(c =>
            c.PhoneNumber == phoneNumber &&
            (!excludeCustomerId.HasValue || c.CustomerId != excludeCustomerId.Value));
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public async Task<List<Customer>> GetPendingCustomersAsync()
    {
        return await _context.Customers.Where(c => c.VerificationStatus == CustomerVerificationStatus.Pending)
            .ToListAsync();
    }
}