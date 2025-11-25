using BankingSystemAPI.Core.DTOs.Request;
using BankingSystemAPI.Core.DTOs.Response;
using BankingSystemAPI.Core.Entities;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Repositories;
using BankingSystemAPI.Core.Interfaces.Services;
using BankingSystemAPI.Core.settings;
using BankingSystemAPI.Services.Helpers;
using BankingSystemAPI.Services.Mappings;
using Microsoft.Extensions.Options;

namespace BankingSystemAPI.Services.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailVerificationTokenService _emailVerificationTokenService;
    private readonly IEmailService _emailService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly RefreshTokenSetting _refreshTokenSetting;
    public AuthenticationService(IUserRepository userRepository, IEmailVerificationTokenService emailVerificationTokenService, IEmailService emailService, JwtTokenGenerator jwtTokenGenerator, IRefreshTokenRepository refreshTokenRepository, IOptions<RefreshTokenSetting> refreshTokenSetting)
    {
        _userRepository = userRepository;
        _emailVerificationTokenService = emailVerificationTokenService;
        _emailService = emailService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenSetting = refreshTokenSetting.Value;
    }
    // Register new user
    public async Task<UserResponseDto> RegisterAsync(UserCreateDto dto)
    {
        if (await _userRepository.GetByEmailAsync(dto.Email) != null)
            throw new Exception("Email already exists.");
        
        var (hash, salt) = PasswordHasher.HashPassword(dto.Password);

        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsEmailVerified = false,
        };

        await _userRepository.AddAsync(newUser);

        var token = await _emailVerificationTokenService.GenerateAndSaveTokenAsync(newUser.UserId, TokenType.EmailVerification, TimeSpan.FromHours(24));
        
        await _emailService.SendVerificationEmailAsync(newUser.Email, token.Token);
        return UserMapper.ToUserResponseDto(newUser);
    }

    //Authenticate or Login
    public async Task<AuthResponseDto?> AuthenticateAsync(string identifier, string password)
    {
        //try to find by email first 
        // If not found, try username
        var user = await _userRepository.GetByEmailAsync(identifier) ?? await _userRepository.GetByUsernameAsync(identifier);
        if (user == null)
            return null;

        var verified = PasswordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        if (!verified)
            return null;

        //generate access token
        var accessToke = _jwtTokenGenerator.GenerateToken(user);
        
        //generate refresh token
        var refreshToken = new RefreshToken
        {
            UserId = user.UserId,
            Token = TokenGenerator.GenerateToken(_refreshTokenSetting.TokenLength),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenSetting.ExpiryDays)
        };
        await _refreshTokenRepository.AddAsync(refreshToken);

        var userDto = UserMapper.ToAuthResponseDto(user, accessToke, refreshToken);

        return userDto;
    }
    
    //Verify email
    public async Task<ResultDto> VerifyEmailAsync(string token)
    {
        var emailToken = await _emailVerificationTokenService.ValidateTokenAsync(token, TokenType.EmailVerification);
        if (emailToken == null || emailToken.ExpiryDate < DateTime.UtcNow || emailToken.UsedAt != null)
            return new ResultDto{ Success = false, Message = "Invalid or expired token." };

        var user = await _userRepository.GetByIdAsync(emailToken.UserId);
        if (user == null)
        {
            return new ResultDto { Success = false, Message = "User not found." };
        }

        user.IsEmailVerified = true;
        
        await _userRepository.UpdateAsync(user);
        
        //Mark as used 
        await _emailVerificationTokenService.MarkTokenUsedAsync(emailToken);
        return new ResultDto { Success = true, Message = "Email verified successfully." };
    }
    
    //Refresh token
    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByRefreshTokenAsync(refreshToken);
        if (storedToken is not { IsActive: true })  //if (storedToken == null || !storedToken.IsActive)
            return null;

        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null)
            return null;
        
        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        
        var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
        var newRefreshToken = new RefreshToken
        {
            UserId = user.UserId,
            Token = TokenGenerator.GenerateToken(_refreshTokenSetting.TokenLength),
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenSetting.ExpiryDays)
        };
        await _refreshTokenRepository.AddAsync(newRefreshToken);
        
        return UserMapper.ToAuthResponseDto(user, newAccessToken, newRefreshToken);
    }
    
    //Logout
    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByRefreshTokenAsync(refreshToken);
        if (storedToken == null || !storedToken.IsActive)
            return false; // nothing to revoke
        
        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        
        return true;
    }

    public async Task<ResultDto> RequestPasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is not { IsEmailVerified: true })
            return new ResultDto { Success = false, Message = "Email not found." };

        var userId = user.UserId;
        var token = await _emailVerificationTokenService.GenerateAndSaveTokenAsync(userId, TokenType.PasswordReset, TimeSpan.FromMinutes(30));
        await _emailService.SendResetPasswordEmailAsync(email, token.Token);
        
        return new ResultDto { Success = true, Message = "If your email is registered, you will receive a password reset link shortly." };
    }
    
    // In your AuthService
    public async Task<bool> ValidatePasswordResetTokenAsync(string token)
    {
        var tokenData = await _emailVerificationTokenService
            .ValidateTokenAsync(token, TokenType.PasswordReset);
    
        return tokenData != null;
    }
    
    //Change password 
    public async Task<ResultDto> ResetPasswordAsync(string token, string newPassword)
    {
        var resetToken = await _emailVerificationTokenService.ValidateTokenAsync(token, TokenType.PasswordReset);
        if (resetToken == null || resetToken.ExpiryDate < DateTime.UtcNow || resetToken.UsedAt != null)
            return new ResultDto { Success = false, Message = "Invalid or expired token." };
        var userId = resetToken.UserId;
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return new ResultDto { Success = false, Message = "User not found." };

        
        // Hash new password
        var (newHash, newSalt) = PasswordHasher.HashPassword(newPassword);
        user.PasswordHash = newHash;
        user.PasswordSalt = newSalt;
        
        await _userRepository.UpdateAsync(user);
        return new ResultDto { Success = true, Message = "Password changed successfully." };
    }
    
}