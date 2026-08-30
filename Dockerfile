# O estágio de SDK existe apenas para restaurar e publicar; ele não aumenta a imagem final.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar os projetos antes do código permite reutilizar o cache de restore quando só arquivos .cs mudam.
COPY global.json Directory.Build.props Directory.Packages.props ./
COPY src/OrderManagement.Domain/OrderManagement.Domain.csproj src/OrderManagement.Domain/
COPY src/OrderManagement.Application/OrderManagement.Application.csproj src/OrderManagement.Application/
COPY src/OrderManagement.Infrastructure/OrderManagement.Infrastructure.csproj src/OrderManagement.Infrastructure/
COPY src/OrderManagement.Api/OrderManagement.Api.csproj src/OrderManagement.Api/

RUN dotnet restore src/OrderManagement.Api/OrderManagement.Api.csproj

COPY src/ src/

RUN dotnet publish src/OrderManagement.Api/OrderManagement.Api.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# O usuário não-root precisa escrever somente no diretório persistido do SQLite.
RUN mkdir -p /app/data && chown -R app:app /app

COPY --from=build --chown=app:app /app/publish .

USER app

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "OrderManagement.Api.dll"]
