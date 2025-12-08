using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.Enums;
using BankingSystemAPI.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AdminController;

    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Administrator")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        private int AdminId => int.Parse(User.FindFirst("id")!.Value);

        //Get all users profile
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _userService.GetAllAsync());
        }

        //Get user profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userService.GetByIdAsync(AdminId);
            return user == null ? NotFound() : Ok(user);
        }

        //Update user
        [HttpPut("{userId:int}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserUpdateDto dto)
        {
            var result = await _userService.UpdateAsync(userId, dto);
            return Ok(result);
        }

        //Delete user
        [HttpDelete("{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var result = await _userService.DeleteAsync(userId, UserRole.Administrator);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
    