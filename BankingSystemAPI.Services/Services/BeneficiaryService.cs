using BankingSystemAPI.Core.DTOs.Request.BeneficiaryRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Beneficiary;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Core.settings;
using BankingSystemAPI.Services.Mappings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BankingSystemAPI.Services.Services;

public class BeneficiaryService : IBeneficiaryService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IBeneficiariesRepo _beneficiariesRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAccountService _accountService;
    private readonly ILogger<BeneficiaryService> _logger;
    private readonly BankSettings _bank;

    public BeneficiaryService(
        ICustomerRepository customerRepo,
        IBeneficiariesRepo beneficiariesRepo,
        IUnitOfWork uow,
        IAccountService accountService,
        ILogger<BeneficiaryService> logger,
        IOptions<BankSettings> bankSettings)
    {
        _customerRepo = customerRepo;
        _beneficiariesRepo = beneficiariesRepo;
        _uow = uow;
        _accountService = accountService;
        _logger = logger;
        _bank = bankSettings.Value;
    }

    public async Task<Result<BeneficiaryDto>> CreateAsync(int userId, CreateBeneficiaryDto dto)
    {
        var isOurBank = dto.BankCode.Equals(_bank.OurBankCode, StringComparison.OrdinalIgnoreCase);
        var customer = await _customerRepo.GetByUserIdAsync(userId);
        if (customer is null)
            return Result<BeneficiaryDto>.Fail("Customer not found");
        var customerId = customer.CustomerId;

        if (await _beneficiariesRepo.ExistsAsync(customerId, dto.AccountNumber, dto.BankCode))
            return Result<BeneficiaryDto>.Fail("Beneficiary already exists.");

        var entity = new Beneficiary
        {
            CustomerId = customerId,
            BeneficiaryName = dto.BeneficiaryName,
            AccountNumber = dto.AccountNumber,
            BankCode = dto.BankCode,
            BankName = isOurBank ? _bank.OurBankName : dto.BankName ?? "Unknown Bank",
            Nickname = dto.Nickname
        };

        await _beneficiariesRepo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var resultDto = BeneficiaryMapper.MapToBeneficiaryDto(entity, _bank.OurBankCode);
        return Result<BeneficiaryDto>.SuccessResult(resultDto);
    }

    public async Task<Result<IEnumerable<BeneficiaryDto>>> GetCustomerBeneficiariesAsync(int customerId)
    {
        var list = await _beneficiariesRepo.GetByCustomerAsync(customerId);

        var dto = list.Select(x => BeneficiaryMapper
                .MapToBeneficiaryDto(x, _bank.OurBankCode))
            .ToList();
        return Result<IEnumerable<BeneficiaryDto>>.SuccessResult(dto);
    }

    public async Task<Result<bool>> DeleteAsync(int customerId, int beneficiaryId)
    {
        var entity = await _beneficiariesRepo.GetByIdAsync(beneficiaryId);

        if (entity is null || entity.CustomerId != customerId)
            return Result<bool>.Fail("Beneficiary not found.");

        _beneficiariesRepo.Remove(entity);
        await _uow.SaveChangesAsync();

        return Result<bool>.SuccessResult(true);
    }

    public async Task<Result<BeneficiaryDto>> SaveBeneficiaryFromTransferAsync(int userId, string reference)
    {
        var receiptResult = await _accountService.GetTransferByReferenceAsync(userId, reference);
        var customer = await _customerRepo.GetByUserIdAsync(userId);
        if (customer is null)
            return Result<BeneficiaryDto>.Fail("Customer not found");
        var customerId =  customer.CustomerId;

        if (!receiptResult.Success || receiptResult.Data is null)
            return Result<BeneficiaryDto>.Fail("Transfer not found");

        var receipt = receiptResult.Data;
        
        // Must be the receiver
        if (receipt.ReceiverCustomerId != customerId)
            return Result<BeneficiaryDto>.Fail("Only receiver can save sender as beneficiary");

        if (await _beneficiariesRepo.ExistsAsync(customerId, receipt.SenderAccountNumber, _bank.OurBankCode))
        {
            return Result<BeneficiaryDto>.Fail("Beneficiary already exists");
        }

        var entity = new Beneficiary
        {
            CustomerId = customerId,
            BeneficiaryName = receipt.SenderName,
            AccountNumber = receipt.SenderAccountNumber,
            BankCode = _bank.OurBankCode,
            BankName = _bank.OurBankName,
            Nickname = customer.FirstName
        };

        await _beneficiariesRepo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var dto = BeneficiaryMapper.MapToBeneficiaryDto(entity, _bank.OurBankCode);

        return Result<BeneficiaryDto>.SuccessResult(dto);
    }
}