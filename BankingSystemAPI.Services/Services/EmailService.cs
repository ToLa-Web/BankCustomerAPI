using System.Net;
using System.Net.Mail;
using BankingSystemAPI.Core.Interfaces.Services;
using BankingSystemAPI.Core.settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BankingSystemAPI.Services.Services;

public class EmailService : IEmailService
{
    private readonly string _fromEmail;
    private readonly string _appPassword;
    private readonly string _baseUrl;

    public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration config)
    {
        _fromEmail = emailSettings.Value.FromEmail;
        _appPassword = emailSettings.Value.AppPassword;
        _baseUrl = config["Application:BaseUrl"]!;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mail = new MailMessage();
        mail.From = new MailAddress(_fromEmail);
        mail.To.Add(toEmail);
        mail.Subject = subject;
        mail.Body = body;
        mail.IsBodyHtml = true;

        using var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(_fromEmail, _appPassword),
            EnableSsl = true
        };

        await smtp.SendMailAsync(mail);
    }

    public async Task SendVerificationEmailAsync(string toEmail, string token)
    {
        var verificationLink = $"{_baseUrl}/api/auth/verify-email?token={Uri.EscapeDataString(token)}";
        var subject = "Verify Your Email Address";
        var body = $@"
            <h2>Welcome to Banking System API!</h2>
            <p>Please click the link below to verify your email:</p>
            <a href='{verificationLink}'>Verify Email</a>
            <p>This link will expire in 24 hours.</p>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendResetPasswordEmailAsync(string toEmail, string token)
    {
        var resetLink = $"{_baseUrl}/api/auth/reset-password?token={Uri.EscapeDataString(token)}";
        var subject = "Reset Your Password";
        var body = $@"
        <h2>Password Reset Request</h2>
        <p>You requested to reset your password. Click the link below to continue:</p>
        <a href='{resetLink}'>Reset Password</a>
        <p>This link will expire in 15 minutes.</p>
        <p>If you didn't request this, please ignore this email and your password will remain unchanged.</p>
    ";

        await SendEmailAsync(toEmail, subject, body);
    }
    
    public async Task SendCustomerApprovedEmailAsync(string toEmail, string fullName)
    {
        var subject = "Your Account Verification is Approved";
        var body = $@"
        <h2>Hello {fullName},</h2>
        <p>We are pleased to inform you that your verification has been completed successfully.</p>
        <p>You now have full access to the banking system.</p>
        <br />
        <p>Regards,</p>
        <p>Banking System Team</p>
    ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendCustomerRejectedEmailAsync(string toEmail, string fullName)
    {
        var subject = "Your Account Verification has been Rejected";
        var body = $@"
        <h2>Hello {fullName},</h2>
        <p>Unfortunately, we are unable to approve your verification request at this time.</p>
        <p>Please recheck your information or contact support for assistance.</p>
        <br />
        <p>Regards,</p>
        <p>Banking System Team</p>
    ";

        await SendEmailAsync(toEmail, subject, body);
    }

}