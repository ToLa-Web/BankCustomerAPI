using BankingSystemAPI.Core.DTOs.Request;
using BankingSystemAPI.Core.DTOs.Response;

namespace BankingSystemAPI.Core.Interfaces.Services;

public interface IAuthenticationService
{
    Task<UserResponseDto> RegisterAsync(UserCreateDto dto);
    Task<UserResponseDto?> AuthenticateAsync(string identifier, string password);
    Task<ResultDto> VerifyEmailAsync(string token);
    Task<ResultDto> RequestPasswordResetAsync(string email);
    Task<bool> ValidatePasswordResetTokenAsync(string token);
    Task<ResultDto> ResetPasswordAsync(string token, string newPassword);
}