using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Authentication;
using OrderManagement.Application.Common;

namespace OrderManagement.UnitTests.Authentication;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsToken()
    {
        var expiresAt = new DateTime(2026, 8, 30, 14, 0, 0, DateTimeKind.Utc);
        var handler = new LoginCommandHandler(
            new StubAuthenticationService(new AuthenticationToken("token", expiresAt)));

        var response = await handler.Handle(
            new LoginCommand("dev@martech.com", "Senha@123"),
            CancellationToken.None);

        Assert.Equal("token", response.AccessToken);
        Assert.Equal(expiresAt, response.ExpiresAt);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ThrowsAuthenticationFailedException()
    {
        var handler = new LoginCommandHandler(new StubAuthenticationService(null));

        await Assert.ThrowsAsync<AuthenticationFailedException>(() => handler.Handle(
            new LoginCommand("invalid@example.com", "invalid"),
            CancellationToken.None));
    }

    private sealed class StubAuthenticationService(AuthenticationToken? token)
        : IAuthenticationService
    {
        public AuthenticationToken? Authenticate(string email, string password) => token;
    }
}
