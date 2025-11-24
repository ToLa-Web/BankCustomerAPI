using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Services.Mappings;

public class UserMapper
{
    public static UserResponseDto ToDto(User user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsEmailVerified = user.IsEmailVerified,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}