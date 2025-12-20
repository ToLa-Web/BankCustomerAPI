using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AdminController;

[ApiController]
[Route("api/admin/audit-logs")]
[Authorize(Roles = "Administrator")]
public class AdminAuditController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AdminAuditController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int? userId,
        [FromQuery] string? action,
        [FromQuery] string? ip,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var (logs, total) = await _auditLogService.GetLogsAsync(
            userId, action, ip, from, to, page, pageSize);

        return Ok(new {
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            Logs = logs
        });
    }
}
