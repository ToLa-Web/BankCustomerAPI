using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BankingSystemAPI.Data.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly BankingSystemDbContext _context;
    public AuditLogRepository(BankingSystemDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AuditLog log)
    {
        await _context.AuditLogs.AddAsync(log);
        //await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetLogsAsync(
        int? userId,
        string? action,
        string? ip,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize
    )
    {
        var query = _context.AuditLogs.AsQueryable();

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId);
        
        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action.Contains(action));

        if (!string.IsNullOrWhiteSpace(ip))
            query = query.Where(a => a.IpAddress!.Contains(ip));
        
        if (from.HasValue)
            query = query.Where(a => a.Timestamp >= from.Value);
        
        if (to.HasValue)
            query = query.Where(a => a.Timestamp <= to.Value);

        var totalCount = await query.CountAsync();
        
        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (logs, totalCount);
    }
}