using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers;

[Authorize(Roles = "Administrator")]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuditLogService _auditLogService;
    public AdminController(IUserService userService,  IAuditLogService auditLogService)
    {
        _userService = userService;
        _auditLogService = auditLogService;
    }
    
    //Get all users
    [Authorize(Roles = "Administrator")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    //Delete user performedByRole
    [Authorize(Roles = "Administrator,Employee")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var roleClaim = User.FindFirst("role")?.Value;
        if(string.IsNullOrEmpty(roleClaim))
            return Unauthorized(new { Message = "Role not found in token." });

        var performedByRole = Enum.Parse<UserRole>(roleClaim);

        var result = await _userService.DeleteAsync(userId, performedByRole);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // Admin: View Audit Logs
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int? userId,
        [FromQuery] string? action,
        [FromQuery] string? ip,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var (logs, totalCount) = await _auditLogService.GetLogsAsync(
            userId, action, ip, from, to, page, pageSize);

        return Ok(new
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Logs = logs
        });
    }
}