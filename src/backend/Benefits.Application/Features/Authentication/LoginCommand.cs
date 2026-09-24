using Benefits.Application.Authentication.DTOs;
using MediatR;

namespace Benefits.Application.Features.Authentication
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
}
