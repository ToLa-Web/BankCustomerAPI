using BankingSystemAPI.Core.DTOs.Request;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Core.Interfaces.Services;
using BankingSystemAPI.Services.Helpers;
using BankingSystemAPI.Services.Mappings;

namespace BankingSystemAPI.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;

    public UserService(IUserRepository userRepository, JwtTokenGenerator  jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    // // Register new user
    // public async Task<UserResponseDto> RegisterAsync(UserCreateDto dto)
    // {
    //     var existing = await _userRepository.GetByEmailAsync(dto.Email);
    //     if (existing != null)
    //     {
    //         throw new InvalidOperationException("Email already registered.");
    //     }
    //
    //     var (hash, salt) = PasswordHasher.HashPassword(dto.Password);
    //     
    //     var user = new User
    //     {
    //         Username = dto.Username,
    //         Email = dto.Email,
    //         PasswordHash = hash,
    //         PasswordSalt = salt,
    //         Role = UserRole.Customer,
    //         IsActive = true,
    //         CreatedAt = DateTime.UtcNow
    //     };
    //     
    //     await _userRepository.AddAsync(user);
    //     return UserMapper.ToDto(user);
    // }
    
    // //Authenticate or Login
    // public async Task<UserResponseDto?> AuthenticateAsync(string identifier, string password)
    // {
    //     //try to find by email first 
    //     // If not found, try username
    //     var user = await _userRepository.GetByEmailAsync(identifier) ?? await _userRepository.GetByUsernameAsync(identifier);
    //
    //     if (user == null)
    //         return null;
    //
    //     var verified = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
    //     if (!verified)
    //         return null;
    //
    //     var token = _jwtTokenGenerator.GenerateToken(user);
    //
    //     var userDto = UserMapper.ToDto(user);
    //     userDto.Token = token; // Assign token in MapToResponseDto
    //
    //     return userDto;
    // }

    //Get user by id
    public async Task<UserResponseDto?> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user == null ? throw new KeyNotFoundException("User not found.") : UserMapper.ToUserResponseDto(user);
    }
    
    //Get user by email
    public async Task<UserResponseDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? throw new KeyNotFoundException("User not found.") : UserMapper.ToUserResponseDto(user);
    }
    
    //Get all user 
    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(UserMapper.ToUserResponseDto);
    }
    
    //User update
    public async Task<UserResponseDto?> UpdateAsync(int userId, UserUpdateDto dto)
    {
        var existing = await _userRepository.GetByIdAsync(userId);
        if (existing == null) 
            throw new KeyNotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Username))
            existing.Username = dto.Username;
        
        if (!string.IsNullOrWhiteSpace(dto.Email))
            existing.Email = dto.Email;
        
        if (dto.Role.HasValue)
            existing.Role = dto.Role.Value;
        
        if (dto.IsActive.HasValue)
            existing.IsActive = dto.IsActive.Value;

        await _userRepository.UpdateAsync(existing);
        
        return UserMapper.ToUserResponseDto(existing);
    }
    
    // //Verify email
    // public async Task<ResultDto> VerifyEmailAsync(int userId, VerifyEmailDto dto)
    // {
    //     var user = await _userRepository.GetByIdAsync(userId);
    //     if (user == null)
    //         return new ResultDto { Success = false, Message = "User not found." };
    //     
    //     if (user.IsEmailVerified)
    //         return new ResultDto { Success = false, Message = "Email already verified." };
    //
    //     if (dto.Token != user.VerificationToken)
    //         return new ResultDto { Success = false, Message = "Invalid or expired token." };
    //
    //     user.IsEmailVerified = true;
    //     user.VerificationToken = null; // optional: clear token after verification
    //     
    //     await _userRepository.UpdateAsync(user);
    //     return new ResultDto { Success = true, Message = "Email verified successfully." };
    // }
    
    // //Change password 
    // public async Task<ResultDto> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    // {
    //     var user = await _userRepository.GetByIdAsync(userId);
    //     if (user == null)
    //         return new ResultDto { Success = false, Message = "User not found." };
    //
    //     // Verify current password using your helper
    //     if (!PasswordHasher.VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
    //         return new ResultDto { Success = false, Message = "Current password is incorrect" };
    //     
    //     // Hash new password
    //     var (newHash, newSalt) = PasswordHasher.HashPassword(dto.NewPassword);
    //     user.PasswordHash = newHash;
    //     user.PasswordSalt = newSalt;
    //     
    //     await _userRepository.UpdateAsync(user);
    //     return new ResultDto { Success = true, Message = "Password changed successfully." };
    // }
    
    //Delete user performedByRole
    public async Task<ResultDto> DeleteAsync(int userId, UserRole performedByRole)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user ==null) 
            return new ResultDto { Success = false, Message = "User not found." };

        switch (performedByRole)
        {
            case UserRole.Administrator:
            {
                //Hard delete
                var deleted = await _userRepository.DeleteAsync(userId);
                return deleted 
                    ? new ResultDto { Success = true, Message = "User permanently deleted." }
                    : new ResultDto { Success = false, Message = "User not found." };
            }
            case UserRole.Employee:
                //Soft delete
                user.IsDeleted = false;
                await _userRepository.UpdateAsync(user);
                return new ResultDto { Success = true, Message = "User deleted successfully." };
            case UserRole.Customer:
            default:
                return new ResultDto { Success = false, Message = "You do not have permission to delete users." };
        }
    }

    // //Helper Mapper 
    // private static UserResponseDto MapToResponseDto(User user)
    // {
    //     return new UserResponseDto
    //     {
    //         UserId = user.UserId,
    //         Username = user.Username,
    //         Email = user.Email,
    //         Role = user.Role,
    //         IsEmailVerified = user.IsEmailVerified,
    //         IsActive = user.IsActive,
    //         CreatedAt = user.CreatedAt,
    //         LastLoginAt =  user.LastLoginAt,
    //     };
    // }
}