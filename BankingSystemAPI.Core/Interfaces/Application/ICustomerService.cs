using BankingSystemAPI.Core.DTOs.Request.CustomerRequest;
using BankingSystemAPI.Core.DTOs.Response;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface ICustomerService
{
    Task<Result<CustomerDto>> CreateCustomerProfileAsync(int userId, CreateCustomerDto dto, string? ip, string? device);
    Task<Result<CustomerDto>> UpdateCustomerProfileAsync(int userId, UpdateCustomerDto dto, string? ip, string? device);
    Task<Result<CustomerDto>> GetCustomerProfileAsync(int userId);
    // admin and staff
    Task<Result<List<CustomerDto>>> GetAllCustomersAsync();
    Task<Result<CustomerDto>> GetCustomerByIdAsync(int customerId);
    Task<Result<CustomerDto>> ApproveCustomerAsync(int customerId, int performedByUserId);
    Task<Result<CustomerDto>> RejectCustomerAsync(int customerId, int performedByUserId);
    Task<Result<CustomerDto>> SuspendCustomerAsync(int customerId, int performedByUserId);
    Task<Result<CustomerDto>> ActivateCustomerAsync(int customerId, int performedByUserId);
    Task<Result<CustomerDto>> CloseCustomerAsync(int customerId, int performedByUserId);
}