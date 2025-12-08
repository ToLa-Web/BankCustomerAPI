namespace BankingSystemAPI.Core.DTOs.Request.AuthRequest;

public class LogoutRequestDto
{
    public string RefreshToken { set; get; } = string.Empty;
}