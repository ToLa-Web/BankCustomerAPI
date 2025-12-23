using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.DTOs.Response.User;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces;
using BankingSystemAPI.Core.Interfaces.Application;
using BankingSystemAPI.Core.Interfaces.Persistence;
using BankingSystemAPI.Services.Mappings;

namespace BankingSystemAPI.Services.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IAuditLogService auditLogService,  IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

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
    
    //User update profile
    public async Task<UserResponseDto?> UpdateProfileAsync(int userId, UserSelfUpdateDto dto)
    {
        var existing = await _userRepository.GetByIdAsync(userId);
        if (existing == null) 
            throw new KeyNotFoundException("User not found.");

        if (!string.IsNullOrWhiteSpace(dto.Username))
            existing.Username = dto.Username;

        await _userRepository.UpdateAsync(existing);
        
        return UserMapper.ToUserResponseDto(existing);
    }
    
    //Update User profile by admin or staff
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

    public async Task<ResultDto> ChangeUserRoleAsync(int userId, ChangeRoleDto dto, int adminId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Role = dto.NewRole;
        user.TokenVersion++;
        
        await _auditLogService.LogAsync(adminId, $"Change user role by admin Id: {adminId}", null, null);
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        return new ResultDto { Success = true, Message = "User successfully changed." };
    }
    
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
            case UserRole.Staff:
                //Soft delete
                user.IsDeleted = false;
                await _userRepository.UpdateAsync(user);
                return new ResultDto { Success = true, Message = "User deleted successfully." };
            case UserRole.Customer:
            default:
                return new ResultDto { Success = false, Message = "You do not have permission to delete users." };
        }
    }

}