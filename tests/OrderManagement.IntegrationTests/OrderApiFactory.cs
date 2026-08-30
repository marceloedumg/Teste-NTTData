using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.IntegrationTests;

/// <summary>
/// Hospeda a API real e troca somente o banco para que os testes validem o pipeline completo com isolamento.
/// </summary>
public sealed class OrderApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"orders-integration-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Um arquivo exclusivo preserva o comportamento real do SQLite sem compartilhar dados entre execuções.
            services.RemoveAll<OrdersDbContext>();
            services.RemoveAll<DbContextOptions<OrdersDbContext>>();

            services.AddDbContext<OrdersDbContext>(options =>
                options.UseSqlite($"Data Source={_databasePath}"));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // SQLite pode criar arquivos auxiliares; todos são removidos para não deixar estado no workspace.
            DeleteIfExists(_databasePath);
            DeleteIfExists($"{_databasePath}-shm");
            DeleteIfExists($"{_databasePath}-wal");
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
