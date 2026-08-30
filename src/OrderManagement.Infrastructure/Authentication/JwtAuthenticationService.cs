using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.Application.Abstractions;

namespace OrderManagement.Infrastructure.Authentication;

/// <summary>
/// Implementa o usuário fixo solicitado e a emissão técnica do JWT fora das regras de negócio.
/// Em produção, esta implementação pode ser trocada sem alterar handlers ou domínio.
/// </summary>
internal sealed class JwtAuthenticationService(
    JwtOptions options,
    TimeProvider timeProvider) : IAuthenticationService
{
    private const string FixedEmail = "dev@martech.com";
    private const string FixedPassword = "Senha@123";

    public AuthenticationToken? Authenticate(string email, string password)
    {
        if (!string.Equals(email, FixedEmail, StringComparison.OrdinalIgnoreCase)
            || !PasswordsMatch(password, FixedPassword))
        {
            return null;
        }

        // O mesmo TimeProvider usado pela Application mantém expiração e testes determinísticos.
        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(options.ExpirationMinutes);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, FixedEmail),
            new Claim(JwtRegisteredClaimNames.Email, FixedEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            now.UtcDateTime,
            expiresAt.UtcDateTime,
            new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new AuthenticationToken(serializedToken, expiresAt.UtcDateTime);
    }

    private static bool PasswordsMatch(string provided, string expected)
    {
        var providedBytes = Encoding.UTF8.GetBytes(provided);
        var expectedBytes = Encoding.UTF8.GetBytes(expected);

        // A comparação em tempo constante reduz vazamento de informação por análise de duração.
        return providedBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(providedBytes, expectedBytes);
    }
}
