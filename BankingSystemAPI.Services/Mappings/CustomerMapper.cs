using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Services.Mappings;

public static class CustomerMapper
{
    public static CustomerDto MapToCustomerDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            CustomerNumber = customer.CustomerNumber,
            NationalId = customer.NationalId,
            PhoneNumber = customer.PhoneNumber,
            FullName = $"{customer.FirstName} {customer.LastName}",
            VerificationStatus = (int)customer.VerificationStatus,
            VerificationStatusName = customer.VerificationStatus.ToString(),
            Status = (int)customer.Status,
            StatusName = customer.Status.ToString(),
        };
    }
}