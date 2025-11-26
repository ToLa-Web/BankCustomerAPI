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
    
    //Update user
    [Authorize]
    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto dto)
    {
        var user = await _userService.UpdateAsync(userId, dto);
        return user == null ? NotFound() : Ok(user);
    }
    
}