using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Core.DTOs.Request.AuthRequest;

public class AuthenticateRequestDto
{
    [Required(ErrorMessage = "Identifier is required.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Identifier must be at least 5 characters.")]
    public string Identifier { get; set; } = string.Empty; // Username OR Email

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; } = string.Empty;
}