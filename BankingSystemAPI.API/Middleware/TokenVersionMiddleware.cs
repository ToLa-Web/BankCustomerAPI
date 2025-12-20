using System.Security.Claims;
using BankingSystemAPI.Core.Interfaces.Persistence;

namespace BankingSystemAPI.API.Middleware;

public class TokenVersionMiddleware
{
    private readonly RequestDelegate _next;
    public TokenVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserRepository userRepository)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tokenVersionClaim = context.User.FindFirstValue("tokenVersion");

            if (userIdClaim != null && tokenVersionClaim != null)
            {
                var userId = int.Parse(userIdClaim);
                var user = await userRepository.GetByIdAsync(userId);
                
                // DB deleted or token outdated
                if (user == null || user.TokenVersion.ToString() != tokenVersionClaim)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Session expired. Please log in again.");
                    return;
                }
            }
        }
        await _next(context);
    }
}