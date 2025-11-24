namespace BankingSystemAPI.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string token);
    Task SendResetPasswordEmailAsync(string toEmail, string token);
}