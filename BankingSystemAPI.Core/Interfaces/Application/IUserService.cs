using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.User;
using BankingSystemAPI.Core.Enums;

namespace BankingSystemAPI.Core.Interfaces.Application;

public interface IUserService
{
    Task<UserResponseDto?> GetByIdAsync(int userId);
    Task<UserResponseDto?> GetByEmailAsync(string email);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> UpdateProfileAsync(int userId, UserSelfUpdateDto dto);
    Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto);
    Task<ResultDto> ChangeUserRoleAsync(int userId, ChangeRoleDto dto, int adminId);
    Task<ResultDto> DeleteAsync(int userId, UserRole performedByRole);
}
    