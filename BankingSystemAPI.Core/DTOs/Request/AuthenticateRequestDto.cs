namespace BankingSystemAPI.Core.DTOs.Request;

public class AuthenticateRequestDto
{
    public string Identifier { get; set; } = string.Empty; // Username or Email
    public string Password { get; set; } = string.Empty;
}