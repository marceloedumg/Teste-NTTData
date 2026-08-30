using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Abstractions;
using OrderManagement.Infrastructure.Authentication;
using OrderManagement.Infrastructure.Orders;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure;

/// <summary>
/// Centraliza a composição das implementações técnicas oferecidas à Application.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra SQLite, repositório específico e autenticação JWT a partir da configuração externa.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrdersDb")
            ?? throw new InvalidOperationException("Connection string 'OrdersDb' was not configured.");

        var jwtOptions = JwtOptions.FromConfiguration(configuration);

        services.AddDbContext<OrdersDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddSingleton(jwtOptions);
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>();

        return services;
    }
}
