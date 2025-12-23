namespace BankingSystemAPI.Core.DTOs.Response.Audilog;

public class AuditLogResponseDto
{
    public int AuditLogId { get; set; }
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Device { get; set; }
    public DateTime Timestamp { get; set; }
}