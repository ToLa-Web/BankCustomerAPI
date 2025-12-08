using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Core.Interfaces.Services;
using UAParser;

namespace BankingSystemAPI.Services.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _logRepository;
    public  AuditLogService(IAuditLogRepository logRepository)
    {
        _logRepository = logRepository;
    }

    public async Task LogAsync(int? userId, string action, string? ip, string? device)
    {
        string readableDevice = "Unknown Device";

        if (!string.IsNullOrWhiteSpace(device))
        {
            try
            {
                var parser = Parser.GetDefault();
                var clientInfo = parser.Parse(device);

                var browser = $"{clientInfo.UA.Family} {clientInfo.UA.Major}";
                var os = clientInfo.OS.Family;
                var deviceFamily = clientInfo.Device.Family;

                // Desktop detection
                if (deviceFamily == "Other" && os.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                    deviceFamily = "Desktop";
                if (deviceFamily == "Other" && os.Contains("Mac", StringComparison.OrdinalIgnoreCase))
                    deviceFamily = "Desktop";
                if (deviceFamily == "Other" && os.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                    deviceFamily = "Desktop";

                // Mobile detection
                if (deviceFamily == "Other" && os.Contains("Android", StringComparison.OrdinalIgnoreCase))
                    deviceFamily = "Mobile";
                if (deviceFamily == "Other" && os.Contains("iOS", StringComparison.OrdinalIgnoreCase))
                    deviceFamily = "Mobile";

                readableDevice = $"{browser} - {os} - {deviceFamily}";
            }
            catch
            {
                readableDevice = "Unknown Device";
            }
        }

        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            IpAddress = ip ?? "Unknown IP",
            Device = readableDevice,
            Timestamp = DateTime.UtcNow
        };

        await _logRepository.AddAsync(log);
    }


    public async Task<(IEnumerable<AuditLogResponseDto> Logs, int TotalCount)> GetLogsAsync(
        int? userId,
        string? action,
        string? ip,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize)
    {
        var (logs, totalCount) = await _logRepository.GetLogsAsync(userId, action, ip, from, to, page, pageSize);

        var logsDto = logs.Select(log => new AuditLogResponseDto
        {
            AuditLogId = log.AuditLogId,
            UserId = log.UserId,
            Action = log.Action,
            IpAddress = log.IpAddress,
            Device = log.Device,
            Timestamp = log.Timestamp
        });
        return (logsDto, totalCount);
    }

}