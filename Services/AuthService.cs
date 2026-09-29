using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Claims;
using Equipment.Api.Models;
using Equipment.Api.Data;
using System.Text;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;
    public AuthService(AppDbContext dbContext, PasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }


    private string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64)
        );
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(us => us.Username == request.Username);

        if (existingUser != null)
        {
            throw new UsernameAlreadyExistsException("Username đã tồn tại");
        }

        var user = new User
        {
            Username = request.Username,
            Role = "User"
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(us => us.Username == request.Username);

        if (user != null)
        {
            var result = _passwordHasher.VerifyHashedPassword(
                                            user,
                                            user.PasswordHash,
                                            request.Password
                                        );
            if (result == PasswordVerificationResult.Success)
            {
                var refreshToken = new RefreshToken
                {
                    Token = CreateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    IsRevoked = false,
                    UserId = user.Id
                };

                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();

                return new AuthResponse
                {
                    Username = user.Username,
                    Role = user.Role,
                    Token = CreateToken(user),
                    RefreshToken = refreshToken.Token
                };
            }
        }
        return null;
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
        {
            return null;
        }

        if (token.IsRevoked || token.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        return new AuthResponse
        {
            Username = token.User.Username,
            Role = token.User.Role,
            Token = CreateToken(token.User),
            RefreshToken = token.Token
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null)
        {
            return false;
        }
        token.IsRevoked = true;

        await _dbContext.SaveChangesAsync();

        return true;
    }
}