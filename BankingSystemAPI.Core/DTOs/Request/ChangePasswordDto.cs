using System.ComponentModel.DataAnnotations;

namespace BankingSystemAPI.Core.DTOs.Request;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters")]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}