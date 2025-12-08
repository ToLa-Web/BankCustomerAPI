namespace BankingSystemAPI.Core.DTOs.Request.CustomerRequest;

using System.ComponentModel.DataAnnotations;

public class CreateCustomerDto
{
    [Required(ErrorMessage = "National ID is required.")]
    [RegularExpression(@"^\d{9}$|^\d{12}$", ErrorMessage = "National ID must be 9 or 12 digits.")]
    public string NationalId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^0\d{7,9}$", ErrorMessage = "Phone must start with 0 and be 8–10 digits.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Date of birth is required.")]
    public DateTime DateOfBirth { get; set; }

    [StringLength(10, ErrorMessage = "Gender must be within 10 characters.")]
    public string? Gender { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string Address { get; set; } = string.Empty;
}
