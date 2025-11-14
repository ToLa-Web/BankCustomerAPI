using BankingSystemAPI.Core.DTOs.Request;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Services;

public interface IUserService
{
    Task<UserResponseDto> RegisterAsync(UserCreateDto dto);
    Task<UserResponseDto?> AuthenticateAsync(string identifier, string password);
    Task<UserResponseDto?> GetByIdAsync(int userId);
    Task<UserResponseDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto);
    Task<ResultDto> VerifyEmailAsync(int userId, VerifyEmailDto dto);
    Task<ResultDto> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<ResultDto> DeleteAsync(int userId, UserRole performedByRole);
}
    