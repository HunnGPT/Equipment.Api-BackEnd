using MediatR;
using Equipment.Api.Domain.Entities;

namespace Equipment.Api.Application.Auth.Commands;

public record RegisterCommand(
    string Username,
    string Password
) : IRequest<User>;