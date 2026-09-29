using Equipment.Api.Models;

public interface IAuthService
{
    Task<User> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
    Task<bool> LogoutAsync(string refreshToken);
}