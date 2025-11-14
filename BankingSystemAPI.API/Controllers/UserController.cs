using BankingSystemAPI.Core.DTOs.Request;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    //
    //Public EndPoint//
    //
    
    //Register 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        var user = await _userService.RegisterAsync(dto);
        return Ok(user);
    }
    
    //Authentication or Login
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequestDto request)
    {
        var user = await _userService.AuthenticateAsync(request.Identifier,  request.Password);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });
        return Ok(user);
    }
    
    //Verify Email
    [HttpPost("{userId}/verify-email")]
    public async Task<IActionResult> VerifyEmail(int userId, [FromBody] VerifyEmailDto dto)
    {
        var result = await _userService.VerifyEmailAsync(userId, dto);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // ------------------------------
    // Protected Endpoints
    // ------------------------------

    //Get user profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
        
        var userId = int.Parse(userIdClaim);
        var user =await _userService.GetByIdAsync(userId);

        return user == null ? NotFound() : Ok(user);
    }

    //Get all users
    [Authorize(Roles = "Administrator")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }
    
    //Update user
    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto dto)
    {
        var user = await _userService.UpdateAsync(userId, dto);
        return user == null ? NotFound() : Ok(user);
    }

    //ChangePassword
    [Authorize]
    [HttpPost("{userId}/change-password")]
    public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
    {
        var result = await _userService.ChangePasswordAsync(userId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    //Delete user performedByRole
    [Authorize(Roles = "Administrator,Employee")]
    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var roleClaim = User.FindFirst("role")?.Value;
        if(string.IsNullOrEmpty(roleClaim))
            return Unauthorized(new { Message = "Role not found in token." });

        var performedByRole = Enum.Parse<UserRole>(roleClaim);

        var result = await _userService.DeleteAsync(userId, performedByRole);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
}