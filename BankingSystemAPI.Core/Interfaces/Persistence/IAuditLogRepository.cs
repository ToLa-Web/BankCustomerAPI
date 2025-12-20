using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Core.Interfaces.Persistence;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log);
    Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetLogsAsync(
        int? userId,
        string? action,
        string? ip,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize
        );
    
}