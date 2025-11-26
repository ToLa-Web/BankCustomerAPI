using BankingSystemAPI.Core.DTOs.Response;

namespace BankingSystemAPI.Core.Interfaces.Services;

public interface IAuditLogService
{
    Task LogAsync(int? userId, string action, string? ip, string? device);
    Task<(IEnumerable<AuditLogResponseDto> Logs, int TotalCount)> GetLogsAsync(
        int? userId,
        string? action,
        string? ip,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize);
}