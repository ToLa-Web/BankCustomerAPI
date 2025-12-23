using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.Auth;
using BankingSystemAPI.Core.DTOs.Response.User;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface IAuthenticationService
{
    Task<UserResponseDto> RegisterAsync(UserCreateDto dto);
    Task<AuthResponseDto?> AuthenticateAsync(string identifier, string password, string? ip,  string? device);
    Task<ResultDto> VerifyEmailAsync(string token);
    Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken, string? ip, string? device);
    Task<bool> LogoutAsync(string refreshToken, string? ip, string? device);
    Task<ResultDto> RequestPasswordResetAsync(string email);
    Task<bool> ValidatePasswordResetTokenAsync(string token);
    Task<ResultDto> ResetPasswordAsync(string token, string newPassword);
}