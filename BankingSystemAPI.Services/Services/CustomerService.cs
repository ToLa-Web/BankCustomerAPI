using BankingSystemAPI.Core.DTOs.Request.CustomerRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Infrastructure;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Services.Mappings;
using BankingSystemAPI.Services.Validators;

namespace BankingSystemAPI.Services.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IAuditLogService  _auditLogService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService  _emailService;

    public CustomerService(
        ICustomerRepository customerRepo, 
        IAuditLogService auditLogService, 
        IUnitOfWork unitOfWork, 
        IEmailService emailService
        )
    {
        _customerRepo = customerRepo;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    // CREATE CUSTOMER PROFILE
    public async Task<Result<CustomerDto>> CreateCustomerProfileAsync(int userId, CreateCustomerDto dto, string? ip, string? device)
    {
        // if user already become a customer
        var exist = await _customerRepo.GetByUserIdAsync(userId);
        if (exist != null)
            return Result<CustomerDto>.Fail("Customer profile already exists.");
        
        // Validate NationalId and Phone number
        var validation = await ValidateCustomerInput(dto);
        if (validation is { Success: false, Message: not null }) 
            return Result<CustomerDto>.Fail(validation.Message);

        var customer = new Customer
        {
            UserId = userId,
            NationalId = dto.NationalId,
            PhoneNumber = dto.PhoneNumber,
            FirstName =  dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth =  dto.DateOfBirth,
            Gender =  dto.Gender,
            Address = dto.Address,
            VerificationStatus = CustomerVerificationStatus.Pending,
            Status = CustomerStatus.Active,
            CustomerNumber = GenerateCustomerNumber()
        };
        
        await _customerRepo.AddAsync(customer);
        await _auditLogService.LogAsync(customer.UserId, "Customer profile created", ip, device);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }

    // UPDATE CUSTOMER PROFILE
    public async Task<Result<CustomerDto>> UpdateCustomerProfileAsync(int userId, UpdateCustomerDto dto, string? ip, string? device)
    {
        var customer = await _customerRepo.GetByUserIdAsync(userId);
        if (customer ==  null)
            return Result<CustomerDto>.Fail("Customer profile not found.");

        if (customer.VerificationStatus == CustomerVerificationStatus.Verified)
        {
            return Result<CustomerDto>.Fail("Cannot change the information after verification.");
        }

        var validation = await ValidateCustomerUpdate(dto, userId);
        if (!validation.Success && validation.Message != null)
            return Result<CustomerDto>.Fail(validation.Message);
        
        // Update allowed fields
        customer.FirstName = dto.FirstName;
        customer.LastName = dto.LastName;
        customer.PhoneNumber = dto.PhoneNumber;
        customer.DateOfBirth = dto.DateOfBirth;
        customer.Gender = dto.Gender;
        customer.Address = dto.Address;
        customer.UpdatedAt = DateTime.Now;
        
        await _customerRepo.UpdateAsync(customer);
        await _auditLogService.LogAsync(customer.UserId, "Customer update profile", ip, device);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }

    // Get Customer profile
    public async Task<Result<CustomerDto>> GetCustomerProfileAsync(int userId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(userId);
        return customer == null ? Result<CustomerDto>.Fail("Customer profile not found.") : 
            Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // Get all customers profile for admin and staff
    public async Task<Result<List<CustomerDto>>> GetAllCustomersAsync()
    {
        var customers = await _customerRepo.GetAllAsync();
        var customerDto = customers.Select(CustomerMapper.MapToCustomerDto).ToList();
        
        return Result<List<CustomerDto>>.SuccessResult(customerDto);
    }
    
    // Get customer profile by customerId
    public async Task<Result<CustomerDto>> GetCustomerByIdAsync(int customerId)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        return customer == null ? 
            Result<CustomerDto>.Fail("Customer not found.") : 
            Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // ApproveCustomer by admin or staff
    public async Task<Result<CustomerDto>> ApproveCustomerAsync(int customerId, int performedByUserId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(customerId);
        if (customer == null)
            return Result<CustomerDto>.Fail("Customer not found.");
        
        if (customer.User == null)
            return Result<CustomerDto>.Fail("User not found.");
        
        if (string.IsNullOrWhiteSpace(customer.User.Email))
            return Result<CustomerDto>.Fail("Customer email is missing.");
        
        customer.VerificationStatus = CustomerVerificationStatus.Verified;
        customer.VerifiedByUserId = performedByUserId;
        customer.VerifiedAt = DateTime.Now;

        await _auditLogService.LogAsync(performedByUserId,
            $"Approved customer id : {customerId} by user id : {performedByUserId}",
            null,
            null);
        await _unitOfWork.SaveChangesAsync();
        
        // Send email to customer 
        await _emailService.SendCustomerApprovedEmailAsync(customer.User.Email,
            $"{customer.FirstName} {customer.LastName}");
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // RejectCustomerAsync by admin or staff
    public async Task<Result<CustomerDto>> RejectCustomerAsync(int customerId, int performedByUserId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(customerId);
        if (customer == null)
            return Result<CustomerDto>.Fail("Customer not found.");
        
        if (customer.User == null)
            return Result<CustomerDto>.Fail("User not found.");
        
        if (string.IsNullOrWhiteSpace(customer.User.Email))
            return Result<CustomerDto>.Fail("Customer email is missing.");
        
        customer.VerificationStatus = CustomerVerificationStatus.Rejected;
        customer.VerifiedByUserId = performedByUserId;
        customer.VerifiedAt = DateTime.Now;
        
        await _auditLogService.LogAsync(performedByUserId, $"Rejected customer id : {customerId} by user id : {performedByUserId}", null, null);
        await _unitOfWork.SaveChangesAsync();
        
        // send email to customer
        await _emailService.SendCustomerRejectedEmailAsync(customer.User.Email,
            $"{customer.FirstName} {customer.LastName}");
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // SuspendCustomer by admin or staff
    public async Task<Result<CustomerDto>> SuspendCustomerAsync(int customerId, int performedByUserId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(customerId);
        if (customer == null)
            return Result<CustomerDto>.Fail("Customer not found.");

        customer.Status = CustomerStatus.Suspended;
        
        await _auditLogService.LogAsync(performedByUserId, $"Suspended customer id : {customerId} by user id : {performedByUserId}", null, null);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // ActivateCustomer by admin or staff
    public async Task<Result<CustomerDto>> ActivateCustomerAsync(int customerId, int performedByUserId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(customerId);
        if (customer == null)
            return Result<CustomerDto>.Fail("Customer not found.");
        
        customer.Status = CustomerStatus.Active;
        
        await _auditLogService.LogAsync(performedByUserId, $"Activated customer id : {customerId} by user id : {performedByUserId}", null, null);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
    
    // CloseCustomer by admin or staff 
    public async Task<Result<CustomerDto>> CloseCustomerAsync(int customerId, int performedByUserId)
    {
        var customer = await _customerRepo.GetByUserIdAsync(customerId);
        if (customer == null)
            return Result<CustomerDto>.Fail("Customer not found.");
        
        customer.Status = CustomerStatus.Closed;
        
        await _auditLogService.LogAsync(performedByUserId, $"Closed customer id : {customerId} by user id {performedByUserId}", null, null);
        await _unitOfWork.SaveChangesAsync();
        
        return Result<CustomerDto>.SuccessResult(CustomerMapper.MapToCustomerDto(customer));
    }
       
    // Helper 
    private async Task<Result<bool>> ValidateCustomerInput(CreateCustomerDto dto)
    {
        // Format validation
        if (!CustomerValidator.IsValidPhoneNumber(dto.PhoneNumber))
            return Result<bool>.Fail("Invalid phone number format.");

        if (!CustomerValidator.IsValidNationalId(dto.NationalId))
            return Result<bool>.Fail("Invalid National ID format.");

        // Uniqueness (database) validation
        if (await _customerRepo.NationalExistAsync(dto.NationalId))
            return Result<bool>.Fail("National ID already exists.");

        if (await _customerRepo.PhoneExistAsync(dto.PhoneNumber))
            return Result<bool>.Fail("Phone number already exists.");

        return Result<bool>.SuccessResult(true);
    }
    
    private async Task<Result<bool>> ValidateCustomerUpdate(UpdateCustomerDto dto, int customerId)
    {
        // format
        if (!CustomerValidator.IsValidPhoneNumber(dto.PhoneNumber))
            return Result<bool>.Fail("Invalid phone number format.");

        // unique check
        if (await _customerRepo.PhoneExistAsync(dto.PhoneNumber, customerId))
            return Result<bool>.Fail("Phone number already exists.");

        return Result<bool>.SuccessResult(true);
    }
    
    private static string GenerateCustomerNumber()
    {
        return "CUST-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }
}