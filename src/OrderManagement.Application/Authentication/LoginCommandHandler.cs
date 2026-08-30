using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Authentication;

/// <summary>
/// Coordena o caso de uso de login sem conhecer detalhes de JWT ou da origem dos usuários.
/// </summary>
public sealed class LoginCommandHandler(IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var token = authenticationService.Authenticate(request.Email, request.Password)
            ?? throw new AuthenticationFailedException();

        return Task.FromResult(new LoginResponse(token.AccessToken, token.ExpiresAt));
    }
}
