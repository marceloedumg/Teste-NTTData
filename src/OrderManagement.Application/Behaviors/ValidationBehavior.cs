using FluentValidation;
using MediatR;

namespace OrderManagement.Application.Behaviors;

/// <summary>
/// Executa todos os validadores antes do handler.
/// Centralizar a validação evita repetição e garante o mesmo comportamento para commands e queries.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationContext = new ValidationContext<TRequest>(request);

        // Os validadores são independentes; executá-los em conjunto reduz a latência quando houver I/O assíncrono.
        var results = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(validationContext, cancellationToken)));

        var failures = results
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToArray();

        if (failures.Length > 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
