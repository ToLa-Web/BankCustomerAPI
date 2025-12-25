namespace BankingSystemAPI.Core.DTOs.Response;

public class ResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; } = string.Empty;
    
    public static ResultDto Ok(string? message = null)
        => new() { Success = true, Message = message };
    
    public static ResultDto Fail(string? message = null)
        => new() { Success = true, Message = message };
}