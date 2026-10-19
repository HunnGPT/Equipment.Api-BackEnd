using MediatR;
using Microsoft.EntityFrameworkCore;
using Equipment.Api.Infrastructure.Data;

namespace Equipment.Api.Application.Auth.Commands;

public class LogoutCommandHandler
    : IRequestHandler<LogoutCommand, bool>
{
    private readonly AppDbContext _dbContext;

    public LogoutCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);


        if (token == null)
        {
            return false;
        }

        token.IsRevoked = true;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}