using System.Security.Claims;
using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers;
[ApiController]
[Authorize]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _userService.GetByIdAsync(UserId);

        return Ok(result);
    }

    [HttpPut("profile/update")]
    public async Task<IActionResult> UpdateProfile(UserSelfUpdateDto dto)
    {
        var result = await _userService.UpdateProfileAsync(UserId, dto);

        return Ok(result);
    }
}