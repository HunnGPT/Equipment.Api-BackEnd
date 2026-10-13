using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Equipment.Api.DTOs.Errors;
using Equipment.Api.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);

            var response = new AuthResponse
            {
                Username = user.Username,
                Role = user.Role
            };

            return Ok(response);
        }
        catch (UsernameAlreadyExistsException)
        {
            return Conflict(
                ErrorResponseFactory.Create(
                    HttpContext,
                    ErrorCodes.UsernameAlreadyExists,
                    "Tên tài khoản đã tồn tại"
                )
            );
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(
                ErrorResponseFactory.Create(
                    HttpContext,
                    ErrorCodes.InvalidCredentials,
                    "Tên đăng nhập hoặc mật khẩu không đúng"
                )
            );
        }

        Response.Cookies.Append("accessToken", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        Response.Cookies.Append("refreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new
        {
            Username = response.Username,
            Role = response.Role
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(
                ErrorResponseFactory.Create(
                    HttpContext,
                    ErrorCodes.InvalidRefreshToken,
                    "Refresh token không hợp lệ"
                )
            );
        }

        var response = await _authService.RefreshTokenAsync(refreshToken);

        if (response == null)
        {
            return Unauthorized(
                ErrorResponseFactory.Create(
                    HttpContext,
                    ErrorCodes.InvalidRefreshToken,
                    "Refresh token không hợp lệ hoặc đã hết hạn"
                )
            );
        }

        Response.Cookies.Append("accessToken", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token không tồn tại");
        }

        var response = await _authService.LogoutAsync(refreshToken);

        if (!response)
        {
            return Unauthorized("Refresh token không tồn tại");
        }

        Response.Cookies.Delete("accessToken");
        Response.Cookies.Delete("refreshToken");

        return Ok("Đăng xuất thành công");
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Username = User.Identity?.Name,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}