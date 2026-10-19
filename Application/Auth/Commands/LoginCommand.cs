using MediatR;
using Equipment.Api.DTOs;

namespace Equipment.Api.Application.Auth.Commands;

public record LoginCommand(
    string Username,
    string Password
) : IRequest<AuthResponse?>;