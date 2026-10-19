using MediatR;

namespace Equipment.Api.Application.Auth.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<bool>;