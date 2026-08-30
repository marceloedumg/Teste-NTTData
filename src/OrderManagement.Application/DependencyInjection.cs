using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Behaviors;

namespace OrderManagement.Application;

/// <summary>
/// Centraliza o registro dos serviços da Application para manter a composição fora dos casos de uso.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registra handlers, validadores, behaviors e o relógio utilizado pelos casos de uso.
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        // TimeProvider é injetável para que testes controlem o tempo sem criar uma abstração própria.
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
