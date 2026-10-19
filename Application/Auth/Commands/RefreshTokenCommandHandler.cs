using MediatR;
using System.Text;
using Equipment.Api.DTOs;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Equipment.Api.Infrastructure.Data;

namespace Equipment.Api.Application.Auth.Commands;

public class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponse?>
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResponse?> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _dbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(
                rt => rt.Token == request.RefreshToken,
                cancellationToken);


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

    private string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["JwT:Key"]!)
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
}