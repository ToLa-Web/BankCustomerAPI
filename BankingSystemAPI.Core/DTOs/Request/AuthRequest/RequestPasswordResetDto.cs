using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Core.DTOs.Request.AuthRequest;

public class RequestPasswordResetDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}