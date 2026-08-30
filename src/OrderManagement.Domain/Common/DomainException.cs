namespace OrderManagement.Domain.Common;

/// <summary>
/// Representa a violação de uma invariante do domínio.
/// Um tipo próprio permite que as camadas externas traduzam a falha sem acoplar o domínio a HTTP.
/// </summary>
public sealed class DomainException(string message) : Exception(message);
