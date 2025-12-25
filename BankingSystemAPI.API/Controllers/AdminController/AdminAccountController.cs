using System.Security.Claims;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AdminController;

[Authorize(Roles = "Administrator")]
[Route("api/admin/accounts")]
[ApiController]
public class AdminAccountController : ControllerBase
{
    private readonly IAccountService  _accountService;
    private readonly IInterestService _interestService;
    private readonly ICurrencyExchangeService _exchangeService;

    public AdminAccountController(IAccountService accountService, IInterestService interestService, ICurrencyExchangeService exchangeService)
    {
        _accountService = accountService;
        _interestService = interestService;
        _exchangeService = exchangeService;
    }
    private int AdminId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    
    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> CustomerAccount(int customerId, int accountId)
    {
        var result = await _accountService.GetAccountByIdAsync(customerId, accountId);
        return Ok(result);
    }
    
    [HttpGet("accounts-by-optional-filters")]
    public async Task<IActionResult> GetAccounts(int? customerId, bool? isActive)
    {
        var result = await _accountService.GetAccountsAsync(customerId, isActive);
        return Ok(result);
    }
        
    [HttpGet("{accountId:int}/balance")]
    public async Task<IActionResult> GetBalance(int customerId, int accountId)
    {
        var result = await _accountService.GetAccountBalanceAsync(customerId, accountId);
        return Ok(result);
    }

    [HttpGet("get-account-by-account-number")]
    public async Task<IActionResult> GetByAccountNumber(string accountNumber)
    {
        var result = await _accountService.GetAccountByAccountNumberAsync(accountNumber);
        return Ok(result);
    }

    [HttpGet("get-all-inactive-accounts")]
    public async Task<IActionResult> GetAllInactiveAccounts()
    {
        var result = await _accountService.GetInactiveAccounts();
        return Ok(result);
    }

    [HttpGet("accounts-by-account-type-filters")]
    public async Task<IActionResult> GetAccountTypes(int? customerId, AccountType? accountType)
    {
        var result = await _accountService.GetAccountsTypeAsync(customerId, accountType);
        return Ok(result);
    }
    
    [HttpPost("{accountId}/freeze")]
    public async Task<IActionResult> Freeze(int accountId)
    {
        var result = await _accountService.FreezeAccountAsync(accountId, AdminId);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpPost("{accountId}/unfreeze")]
    public async Task<IActionResult> UnFreeze(int accountId)
    {
        var result = await _accountService.UnfreezeAccountAsync(accountId, AdminId);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("interset/apply")]
    public async Task<IActionResult> ApplyInterset()
    {
        var result = await _interestService.ApplyMonthlyInterestAsync();
        return Ok(result);
    }
    
    [HttpGet("exchange/convert")]
    public async Task<IActionResult> Convert(decimal amount, string from, string to)
    {
        var result = await _exchangeService.ConvertAsync(amount, from, to);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    
}