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