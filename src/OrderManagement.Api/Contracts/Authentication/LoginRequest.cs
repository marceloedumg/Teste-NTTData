namespace OrderManagement.Api.Contracts.Authentication;

/// <summary>
/// Contrato HTTP de login separado do command para que a API possa evoluir sem contaminar a Application.
/// </summary>
public sealed record LoginRequest(string Email, string Password);
