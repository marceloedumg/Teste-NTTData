namespace OrderManagement.Application.Common;

/// <summary>
/// Sinaliza credenciais inválidas sem revelar qual campo falhou, evitando enumeração de usuários.
/// </summary>
public sealed class AuthenticationFailedException()
    : Exception("Invalid email or password.");
