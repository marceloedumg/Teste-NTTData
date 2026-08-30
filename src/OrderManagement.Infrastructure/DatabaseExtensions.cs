using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure;

/// <summary>
/// Expõe a inicialização do banco sem obrigar a API a conhecer o DbContext concreto em detalhes.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    /// Aplica somente migrations pendentes para que execução local e Docker iniciem com o schema correto.
    /// </summary>
    public static async Task ApplyDatabaseMigrationsAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        // DbContext é scoped; por isso a inicialização cria e descarta um escopo próprio.
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
