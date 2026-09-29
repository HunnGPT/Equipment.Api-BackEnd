using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

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
        catch (UsernameAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized("Sai tên tài khoản hoặc mật khẩu");
        }
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);

        if (response == null)
        {
            return Unauthorized("Refresh token không hợp lệ hoặc đã hết hạn");
        }

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequest request)
    {
        var response = await _authService.LogoutAsync(request.RefreshToken);
        if (response)
        {
            return Ok("Đăng xuất thành công");
        }
        return Unauthorized("Refresh token không tồn tại");
    }
}