using System.Security.Claims;
using BankingSystemAPI.Core.DTOs.Request.BeneficiaryRequest;
using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.BeneficiaryController;
[Authorize(Roles = "Customer,Administrator")]
[Authorize(Policy = "VerifiedCustomerOnly")]
[Route("api/")]
[ApiController]
public class BeneficiaryController : ControllerBase
{
    private readonly IBeneficiaryService _beneficiaryService;
    public BeneficiaryController (IBeneficiaryService beneficiaryService) => _beneficiaryService = beneficiaryService;
    
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    [HttpPost("from-transfer/{reference}")]
    public async Task<IActionResult> SaveFromTransfer(string reference)
    {
        var result = await _beneficiaryService
            .SaveBeneficiaryFromTransferAsync(UserId, reference);

        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpPost("beneficiary/create")]
    public async Task<IActionResult> CreateBeneficiary(CreateBeneficiaryDto beneficiary)
    {
        var result = await _beneficiaryService.CreateAsync(UserId, beneficiary);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

    [HttpGet("lists")]
    public async Task<IActionResult> GetList()
    {
        var result = await _beneficiaryService.GetCustomerBeneficiariesAsync(UserId);
        return result.Success ? Ok(result.Data) : BadRequest(result);
    }

}