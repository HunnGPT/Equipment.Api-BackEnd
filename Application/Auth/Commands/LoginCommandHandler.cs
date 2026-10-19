using MediatR;
using System.Text;
using Equipment.Api.DTOs;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Equipment.Api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Equipment.Api.Infrastructure.Data;

namespace Equipment.Api.Application.Auth.Commands;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponse?>
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        AppDbContext dbContext,
        PasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(us => us.Username == request.Username, cancellationToken);


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
                await _dbContext.SaveChangesAsync(cancellationToken);

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

    private string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
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
}