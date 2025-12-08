using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Core.DTOs.Request.AuthRequest;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = string.Empty;
    
    [Required(ErrorMessage =  "New Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8,  ErrorMessage = "Password must be at least 8 characters long")]
    public string NewPassword { get; set; } = string.Empty;
    
    // [Required]
    // [DataType(DataType.Password)]
    // [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
    // public string ConfirmPassword { get; set; } = string.Empty;
}