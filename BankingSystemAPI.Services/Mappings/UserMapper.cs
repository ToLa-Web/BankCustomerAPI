using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;

namespace BankingSystemAPI.Services.Mappings;

public class UserMapper
{
    public static UserResponseDto ToUserResponseDto(User user)
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

    public static AuthResponseDto ToAuthResponseDto(User user, string accessToken, RefreshToken refreshToken)
    {
        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive,
            IsEmailVerified = user.IsEmailVerified,
            Token = accessToken,
            RefreshToken = refreshToken.Token,
            LastLoginAt = user.LastLoginAt
        };
    } 
}