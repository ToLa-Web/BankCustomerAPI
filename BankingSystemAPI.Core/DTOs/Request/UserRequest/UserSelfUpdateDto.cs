using System.ComponentModel.DataAnnotations;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Request.UserRequest;

public class UserSelfUpdateDto
{
    [StringLength(50, MinimumLength = 5)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username must contain letters, numbers or underscore only.")]
    public string? Username { get; set; }
    
    //[StringLength(100)]
    //[EmailAddress(ErrorMessage = "Invalid email format.")]
    //public string? Email { get; set; }
}