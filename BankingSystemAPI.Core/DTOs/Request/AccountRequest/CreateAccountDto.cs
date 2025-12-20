using System.ComponentModel.DataAnnotations;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.DTOs.Request.AccountRequest;

public class CreateAccountDto
{
    [Required(ErrorMessage = "Account type is required")]
    public AccountType AccountType { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    [StringLength(3, MinimumLength = 3,
        ErrorMessage = "Currency must be a 3-letter ISO code")]
    [RegularExpression("^[A-Z]{3}$",
        ErrorMessage = "Currency must be uppercase ISO-4217 code (e.g. KHR, USD, EUR)")]
    public string Currency { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "Fixed term must be between 1 and 120 months")]
    public int? FixedTermMonths { get; set; }
}