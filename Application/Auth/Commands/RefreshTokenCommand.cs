using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken)
    : IRequest<AuthResponse?>;