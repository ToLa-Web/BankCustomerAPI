using BankingSystemAPI.Core.DTOs.Request.AuthRequest;
using BankingSystemAPI.Core.DTOs.Request.UserRequest;
using BankingSystemAPI.Core.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.API.Controllers.AuthController;
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authService;
    public AuthController(IAuthenticationService authService)
    {
        _authService = authService;
    }
    
    //Public EndPoint//
    
    //Register 
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserCreateDto dto)
    {
        var user = await _authService.RegisterAsync(dto);
        return Ok(user);
    }
    
    //Authentication or Login
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate(AuthenticateRequestDto request)
    {
        var (ip, device) = GetRequestInfo();

        var result = await _authService.AuthenticateAsync(request.Identifier, request.Password, ip, device);

        return result == null
            ? Unauthorized(new { message = "Invalid credentials." })
            : Ok(result);
    }
    
    //Verify Email
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var result = await _authService.VerifyEmailAsync(token);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    //Request refreshToken
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromQuery] RefreshTokenRequestDto request)
    {
        var (ip, device) = GetRequestInfo();
        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ip, device);
        return result != null ?  Ok(result) : BadRequest(new {message = "Invalid or expired refresh token"});
    }
    
    //Request logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] LogoutRequestDto request)
    {
        var (ip, device) = GetRequestInfo();
        var success = await _authService.LogoutAsync(request.RefreshToken, ip, device);
        if (!success)
            return BadRequest(new {message = "Invalid or expired refresh token"});
        
        return Ok(new {message = "Logout successful"});
    }
    
    //Request reset password
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetDto request)
    {
        var result = await _authService.RequestPasswordResetAsync(request.Email);
        
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    // This handles the email link click
    [HttpGet("reset-password")]
    public async Task<IActionResult> ShowResetPasswordForm([FromQuery] string token)
    {
        // Validate the token
        var isValid = await _authService.ValidatePasswordResetTokenAsync(token);
        
        if (!isValid)
            return BadRequest("Invalid or expired token");
        
        // Return a simple HTML form
        var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Reset Password</title>
                <style>
                    body {{ font-family: Arial; max-width: 400px; margin: 50px auto; padding: 20px; }}
                    input {{ width: 100%; padding: 10px; margin: 10px 0; box-sizing: border-box; }}
                    button {{ width: 100%; padding: 10px; background: #007bff; color: white; border: none; cursor: pointer; }}
                    button:hover {{ background: #0056b3; }}
                    .error {{ color: red; }}
                    .success {{ color: green; }}
                </style>
            </head>
            <body>
                <h2>Reset Your Password</h2>
                <form id='resetForm'>
                    <input type='password' id='newPassword' placeholder='New Password' required minlength='6' />
                    <input type='password' id='confirmPassword' placeholder='Confirm Password' required />
                    <button type='submit'>Reset Password</button>
                </form>
                <div id='message'></div>
                
                <script>
                    document.getElementById('resetForm').addEventListener('submit', async (e) => {{
                        e.preventDefault();
                        const newPassword = document.getElementById('newPassword').value;
                        const confirmPassword = document.getElementById('confirmPassword').value;
                        const messageDiv = document.getElementById('message');
                        
                        if (newPassword !== confirmPassword) {{
                            messageDiv.innerHTML = '<p class=""error"">Passwords do not match!</p>';
                            return;
                        }}
                        
                        try {{
                            const response = await fetch('/api/auth/reset-password', {{
                                method: 'POST',
                                headers: {{ 'Content-Type': 'application/json' }},
                                body: JSON.stringify({{ token: '{token}', newPassword }})
                            }});
                            
                            const result = await response.json();
                            
                            if (response.ok) {{
                                messageDiv.innerHTML = '<p class=""success"">' + result.message + '</p>';
                                setTimeout(() => window.location.href = '/login', 2000);
                            }} else {{
                                messageDiv.innerHTML = '<p class=""error"">' + result.message + '</p>';
                            }}
                        }} catch (error) {{
                            messageDiv.innerHTML = '<p class=""error"">An error occurred. Please try again.</p>';
                        }}
                    }});
                </script>
            </body>
            </html>
        ";
        
        return Content(html, "text/html");
    }
    
    //Reset Password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    //Helper Method 
    private (string? ip, string? device) GetRequestInfo()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var device = HttpContext.Request.Headers.UserAgent.ToString();
        return (ip, device);
    }
}