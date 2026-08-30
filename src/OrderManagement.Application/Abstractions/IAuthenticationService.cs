namespace OrderManagement.Application.Abstractions;

/// <summary>
/// Abstrai a validação de credenciais e a emissão do token.
/// A Application depende do contrato, enquanto JWT e usuário fixo permanecem substituíveis na Infrastructure.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Autentica as credenciais e retorna um token, ou <see langword="null"/> quando forem inválidas.
    /// </summary>
    AuthenticationToken? Authenticate(string email, string password);
}

/// <summary>
/// Resultado independente da tecnologia usada para emitir o token de acesso.
/// </summary>
public sealed record AuthenticationToken(string AccessToken, DateTime ExpiresAt);
