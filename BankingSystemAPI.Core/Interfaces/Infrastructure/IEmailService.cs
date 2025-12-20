namespace BankingSystemAPI.Core.Interfaces.Infrastructure;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string toEmail, string token);
    Task SendResetPasswordEmailAsync(string toEmail, string token);
    Task SendCustomerApprovedEmailAsync(string toEmail, string fullName);
    Task SendCustomerRejectedEmailAsync(string toEmail, string fullName);
}