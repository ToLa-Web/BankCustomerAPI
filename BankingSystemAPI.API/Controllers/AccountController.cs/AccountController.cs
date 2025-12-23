using System.Security.Claims;
using BankingSystemAPI.Core.DTOs.Request.AccountRequest;
using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AccountController.cs;

[Authorize(Roles = "Customer,Administrator")]
[Authorize(Policy = "VerifiedCustomerOnly")]
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    private int CustomerId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto dto)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _accountService.CreateAccountAsync(CustomerId, dto, ip, device);
        return Ok(result);
    }

    [HttpGet("{accountId:int}")]
    public async Task<IActionResult> MyAccount(int accountId)
    {
        var result = await _accountService.GetAccountByIdAsync(CustomerId, accountId);
        return Ok(result);
    }

    [HttpGet("my-accounts")]
    public async Task<IActionResult> GetMyAccounts()
    {
        var result = await _accountService.GetAccountByCustomerAsync(CustomerId);
        return Ok(result);
    }

    [HttpGet("{accountId:int}/balance")]
    public async Task<IActionResult> GetBalance(int accountId)
    {
        var result = await _accountService.GetAccountBalanceAsync(CustomerId, accountId);
        return Ok(result);
    }

    [HttpPost("{id:int}/deposit")]
    public async Task<IActionResult> Deposit(int id, decimal amount)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _accountService.DepositAsync(id, amount, ip, device);
        
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    [HttpPost("{id:int}/withdraw")]
    public async Task<IActionResult> Withdraw(int id, decimal amount)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _accountService.WithdrawAsync(id, amount, ip, device);
        
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }
     
    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer([FromBody] TransferRequestDto transferDto)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _accountService.TransferAsync(CustomerId, transferDto, ip, device);
        
        return result.Success ? Ok(result) : BadRequest(result.Message);
    }

    [HttpGet("{accountId}/transactions")]
    public async Task<IActionResult> GetTransactions(
        int accountId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _accountService.GetAccountTransactionsAsync(CustomerId, accountId, page, pageSize);
        return Ok(result);
    }
     
    
    private (string? ip, string? device) GetRequestInfo()
         {
             var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
             var device = HttpContext.Request.Headers.UserAgent.ToString();
             return (ip, device);
         }
}