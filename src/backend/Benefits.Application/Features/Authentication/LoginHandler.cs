using Benefits.Application.Authentication.DTOs;
using Benefits.Application.Authentication.Interfaces;
using Benefits.Application.Exceptions;
using Benefits.Application.Infrastructure.Contracts;
using Benefits.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Benefits.Application.Features.Authentication
{
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse?>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IBenefitsDbContext _dbContext;

        public LoginHandler(IAuthenticationService authenticationService, IBenefitsDbContext dbContext)
        {
            _authenticationService = Guard.NotNull(authenticationService);
            _dbContext = Guard.NotNull(dbContext);
        }

        public async Task<LoginResponse?> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var loginRequest = new LoginRequest
            {
                Email = command.Email,
                Password = command.Password,
            };

            var loginResponse = await _authenticationService.AuthenticateAsync(loginRequest, cancellationToken);

            if (loginResponse is null)
            {
                return null;
            }

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.UserId == loginResponse.User.Id, cancellationToken)
                ?? throw new NotFoundException($"Employee with email {loginRequest.Email} was not found or does not have a valid security access.");

            loginResponse.User.FirstName = employee.FirstName;
            loginResponse.User.LastName = employee.LastName;

            return loginResponse;
        }
    }
}
