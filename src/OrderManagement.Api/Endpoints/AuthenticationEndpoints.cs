using MediatR;
using OrderManagement.Api.Contracts.Authentication;
using OrderManagement.Application.Authentication;

namespace OrderManagement.Api.Endpoints;

/// <summary>
/// Agrupa o endpoint de autenticação e mantém o arquivo de inicialização como composition root.
/// </summary>
internal static class AuthenticationEndpoints
{
    internal static IEndpointRouteBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/auth/login", LoginAsync)
            .AllowAnonymous()
            .WithTags("Authentication")
            .WithName("Login")
            .Produces<LoginResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        // O endpoint apenas adapta HTTP para MediatR; autenticação e emissão do token ficam fora da API.
        var response = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return Results.Ok(response);
    }
}
