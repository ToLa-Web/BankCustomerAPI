using System.Security.Claims;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AdminController;

[Authorize(Roles = "Administrator")]
[Route("api/admin/accounts")]
[ApiController]
public class AdminAccountController : ControllerBase
{
    private readonly IAccountService  _accountService;

    public AdminAccountController(IAccountService accountService)
    {
        _accountService = accountService;
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
}