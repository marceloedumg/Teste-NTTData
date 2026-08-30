# Order Management API — backend

Snapshot do backend do teste técnico, mantido nesta branch para facilitar a revisão isolada da
solução .NET. A integração completa com frontend e Docker Compose está na branch `main`.

## Stack

- .NET 10 e Minimal APIs;
- Clean Architecture em Domain, Application, Infrastructure e API;
- CQRS com MediatR e validação com FluentValidation;
- EF Core e SQLite com migration automática;
- autenticação JWT, Swagger, Serilog e OpenTelemetry;
- xUnit para testes unitários e de integração.

## Executar

```bash
dotnet restore OrderManagement.slnx
dotnet run --project src/OrderManagement.Api
```

API: `http://localhost:5080`  
Swagger: `http://localhost:5080/swagger`

Credenciais de avaliação:

```text
E-mail: dev@martech.com
Senha:  Senha@123
```

## Testes

```bash
dotnet test OrderManagement.slnx
```

A suíte contém 41 testes unitários e 8 testes de integração. Os cenários cobrem autenticação,
validação, criação, listagem, consulta, cancelamento, erros, CORS e documentação OpenAPI.

## Branches

- `backend`: API e testes isolados;
- `frontend`: adiciona a interface React;
- `main`: produto completo integrado pelo Docker Compose.
