using Microsoft.Extensions.Configuration;

namespace OrderManagement.Infrastructure.Authentication;

/// <summary>
/// Agrupa a configuração necessária para emitir e validar JWTs com os mesmos parâmetros.
/// </summary>
public sealed record JwtOptions(
    string Issuer,
    string Audience,
    string Key,
    int ExpirationMinutes)
{
    /// <summary>Nome da seção de configuração para evitar chaves espalhadas pelo código.</summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Constrói opções válidas no início da aplicação, falhando cedo quando a configuração é insegura.
    /// </summary>
    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        var issuer = configuration[$"{SectionName}:Issuer"];
        var audience = configuration[$"{SectionName}:Audience"];
        var key = configuration[$"{SectionName}:Key"];
        var expirationValue = configuration[$"{SectionName}:ExpirationMinutes"];

        if (string.IsNullOrWhiteSpace(issuer)
            || string.IsNullOrWhiteSpace(audience)
            || string.IsNullOrWhiteSpace(key)
            || !int.TryParse(expirationValue, out var expirationMinutes)
            || expirationMinutes <= 0)
        {
            throw new InvalidOperationException("JWT configuration is missing or invalid.");
        }

        if (key.Length < 32)
        {
            throw new InvalidOperationException("JWT key must contain at least 32 characters.");
        }

        return new JwtOptions(issuer, audience, key, expirationMinutes);
    }
}
