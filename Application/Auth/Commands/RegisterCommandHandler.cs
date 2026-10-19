using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Equipment.Api.Domain.Entities;
using Equipment.Api.Infrastructure.Data;
using Equipment.Api.Application.Auth.Exceptions;

namespace Equipment.Api.Application.Auth.Commands;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, User>
{
    private readonly AppDbContext _dbContext;
    private readonly PasswordHasher<User> _passwordHasher;

    public RegisterCommandHandler(
        AppDbContext dbContext,
        PasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

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
        await _dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }
}