using System.Security.Claims;
using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AdminController;

[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = "Administrator")]
public class AdminCustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public AdminCustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private int AdminId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    // Get all customers
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _customerService.GetAllCustomersAsync());

    // Get customer by CustomerId
    // GET: /api/admin/customers/{customerId}
    [HttpGet("{customerId:int}")]
    public async Task<IActionResult> GetCustomer(int customerId)
        => Ok(await _customerService.GetCustomerByIdAsync(customerId));

    // Approve KYC
    // PUT: /api/admin/customers/{customerId}/approve
    [HttpPut("{customerId:int}/approve")]
    public async Task<IActionResult> Approve(int customerId)
        => Ok(await _customerService.ApproveCustomerAsync(customerId, AdminId));

    // Reject KYC
    // PUT: /api/admin/customers/{customerId}/reject
    [HttpPut("{customerId:int}/reject")]
    public async Task<IActionResult> Reject(int customerId)
        => Ok(await _customerService.RejectCustomerAsync(customerId, AdminId));

    // Suspend account 
    // PUT: /api/admin/customers/{customerId}/suspend
    [HttpPut("{customerId:int}/suspend")]
    public async Task<IActionResult> Suspend(int customerId)
        => Ok(await _customerService.SuspendCustomerAsync(customerId, AdminId));

    // Activate account (optional)
    // PUT: /api/admin/customers/{customerId}/activate
    [HttpPut("{customerId:int}/activate")]
    public async Task<IActionResult> Activate(int customerId)
        => Ok(await _customerService.ActivateCustomerAsync(customerId, AdminId));

    // Close account (optional)
    // PUT: /api/admin/customers/{customerId}/closed
    [HttpPut("{customerId:int}/close")]
    public async Task<IActionResult> Close(int customerId)
        => Ok(await _customerService.CloseCustomerAsync(customerId, AdminId));
}
