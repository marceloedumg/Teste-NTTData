using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Behaviors;

/// <summary>
/// Registra início, fim e duração de cada caso de uso no mesmo pipeline do MediatR.
/// Isso oferece observabilidade uniforme sem poluir handlers com preocupação transversal.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var isSensitive = request is ISensitiveRequest;
        var stopwatch = Stopwatch.StartNew();

        // Login continua observável por nome e duração, mas seu payload e resposta não são registrados.
        if (isSensitive)
        {
            logger.LogInformation("Handling {RequestName} (payload omitted)", requestName);
        }
        else
        {
            logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);
        }

        try
        {
            var response = await next();
            stopwatch.Stop();

            if (isSensitive)
            {
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds} ms (response omitted)",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds} ms: {@Response}",
                    requestName,
                    stopwatch.ElapsedMilliseconds,
                    response);
            }

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            logger.LogWarning(
                exception,
                "Failed {RequestName} after {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}
