using System.Security.Claims;
using BankingSystemAPI.Core.DTOs.Request.CustomerRequest;
using BankingSystemAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }
    
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // CREATE CUSTOMER PROFILE (Onboarding)
    [Authorize]
    [HttpPost("CreateCustomer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
    {
        var (ip, device) = GetRequestInfo();
        var result =await _customerService.CreateCustomerProfileAsync(UserId, customerDto, ip, device);
        
        return result.Success ? Ok(result) : BadRequest();
    }

    // UPDATE MY PROFILE AS CUSTOMER  (Locked after Verification)
    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerDto customerDto)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _customerService.UpdateCustomerProfileAsync(UserId, customerDto, ip, device);
        return Ok(result);
    }

    //GET MY PROFILE AS CUSTOMER
    [Authorize]
    [HttpGet("GetMyProfile")]
    public async Task<IActionResult> GetCustomerProfile()
    {
        var result = await _customerService.GetCustomerProfileAsync(UserId);
        
        return result.Success ? Ok(result) : BadRequest();
    }
    
    //Helper Method 
    private (string? ip, string? device) GetRequestInfo()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var device = HttpContext.Request.Headers.UserAgent.ToString();
        return (ip, device);
    }
    
}