namespace BankingSystemAPI.Core.DTOs.Request;

public class LogoutRequestDto
{
    public string RefreshToken { set; get; } = string.Empty;
}