using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Authentication;

/// <summary>
/// Solicita autenticação pelo mediator para manter o endpoint livre de lógica de aplicação.
/// A marcação sensível impede que senha e token sejam escritos nos logs.
/// </summary>
public sealed record LoginCommand(string Email, string Password)
    : IRequest<LoginResponse>, ISensitiveRequest;

/// <summary>Retorna ao cliente o token emitido e seu instante de expiração.</summary>
public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt);
