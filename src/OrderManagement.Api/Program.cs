using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderManagement.Api.Endpoints;
using OrderManagement.Api.Errors;
using OrderManagement.Api.OpenApi;
using OrderManagement.Application;
using OrderManagement.Infrastructure;
using OrderManagement.Infrastructure.Authentication;
using Serilog;

const string FrontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

// Serilog é configurado no host para unificar logs da aplicação, do ASP.NET Core e dos behaviors.
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// A API é a composition root: conhece as camadas concretas, enquanto o domínio permanece independente.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = JwtOptions.FromConfiguration(builder.Configuration);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var allowedFrontendOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(origin => origin.Value)
    .Where(origin => !string.IsNullOrWhiteSpace(origin))
    .Cast<string>()
    .ToArray();

builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy =>
{
    if (allowedFrontendOrigins.Length == 0)
    {
        // Sem origens configuradas, a política permanece fechada em vez de liberar acesso acidentalmente.
        return;
    }

    // Bearer não usa cookies; restringir a origem já permite headers e métodos necessários ao frontend.
    policy
        .WithOrigins(allowedFrontendOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("Location");
}));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi(options =>
{
    // Os transformers mantêm o contrato fiel: registram JWT e marcam somente operações protegidas.
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddOperationTransformer<AuthorizationOperationTransformer>();
});
builder.Services.AddHealthChecks();

// Enums como texto tornam o contrato HTTP mais legível e menos dependente dos valores numéricos internos.
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// O exporter de console atende ao diagnóstico local e pode ser trocado por OTLP em produção.
builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("OrderManagement.Api"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddConsoleExporter());

var app = builder.Build();

// A ordem garante tratamento uniforme de erros antes da autenticação e autorização dos endpoints.
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();

// A UI consome o documento nativo acima, sem manter uma segunda definição do contrato HTTP.
// Ela fica habilitada no contêiner de avaliação; em produção pública deve ser protegida ou desabilitada.
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "Order Management API v1");
    options.DocumentTitle = "Order Management API";
    options.DisplayRequestDuration();
});

app.MapHealthChecks("/health").AllowAnonymous();
app.MapAuthenticationEndpoints();
app.MapOrderEndpoints();

// Aplicar migrations no startup garante a mesma experiência local e no contêiner solicitada no teste.
await app.Services.ApplyDatabaseMigrationsAsync();

app.Run();

/// <summary>
/// Expõe o ponto de entrada para que o WebApplicationFactory inicialize a aplicação nos testes de integração.
/// </summary>
public partial class Program;
