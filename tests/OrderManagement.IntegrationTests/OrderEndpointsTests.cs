using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OrderManagement.IntegrationTests;

public sealed class OrderEndpointsTests(OrderApiFactory factory)
    : IClassFixture<OrderApiFactory>
{
    [Fact]
    public async Task CorsPreflight_FromConfiguredFrontendOrigin_ReturnsAllowOriginHeader()
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/orders");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "GET");
        request.Headers.Add("Access-Control-Request-Headers", "authorization");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(
            "http://localhost:5173",
            Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));

        using var blockedRequest = new HttpRequestMessage(HttpMethod.Options, "/api/orders");
        blockedRequest.Headers.Add("Origin", "https://not-configured.example");
        blockedRequest.Headers.Add("Access-Control-Request-Method", "GET");

        var blockedResponse = await client.SendAsync(blockedRequest);

        Assert.False(blockedResponse.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task Swagger_ReturnsInteractiveDocumentation()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/index.html");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(
            "swagger-ui",
            await response.Content.ReadAsStringAsync(),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetOrders_WithoutToken_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/orders");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorizedProblem()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/auth/login", new
        {
            email = "invalid@example.com",
            password = "invalid"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task CreateOrder_WithoutItems_ReturnsValidationProblem()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var response = await client.PostAsJsonAsync("/api/orders", new
        {
            customerId = Guid.NewGuid(),
            items = Array.Empty<object>()
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("Items", out _));
    }

    [Fact]
    public async Task ListOrders_WithPageSizeAboveLimit_ReturnsValidationProblem()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/orders?page=1&pageSize=101");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(document.RootElement.GetProperty("errors").TryGetProperty("PageSize", out _));
    }

    [Fact]
    public async Task GetOrder_WhenOrderDoesNotExist_ReturnsNotFoundProblem()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var response = await client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task AuthenticatedOrderFlow_CreateGetListAndCancel_Succeeds()
    {
        using var client = await CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync("/api/orders", new
        {
            customerId = Guid.NewGuid(),
            items = new[]
            {
                new { productName = "Mechanical Keyboard", quantity = 2, unitPrice = 350.50m }
            }
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await ReadOrderAsync(createResponse);
        Assert.Equal("Pending", created.Status);
        Assert.Equal(701m, created.TotalAmount);

        var getResponse = await client.GetAsync($"/api/orders/{created.Id}");
        getResponse.EnsureSuccessStatusCode();
        var fetched = await ReadOrderAsync(getResponse);
        Assert.Equal(created.Id, fetched.Id);

        var listResponse = await client.GetAsync("/api/orders?page=1&pageSize=10");
        listResponse.EnsureSuccessStatusCode();
        using var listDocument = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        Assert.True(listDocument.RootElement.GetProperty("totalCount").GetInt32() >= 1);

        var cancelResponse = await client.PatchAsync(
            $"/api/orders/{created.Id}/cancel",
            content: null);
        Assert.Equal(HttpStatusCode.NoContent, cancelResponse.StatusCode);

        var cancelledResponse = await client.GetAsync($"/api/orders/{created.Id}");
        cancelledResponse.EnsureSuccessStatusCode();
        var cancelled = await ReadOrderAsync(cancelledResponse);
        Assert.Equal("Cancelled", cancelled.Status);

        // A segunda tentativa comprova a regra observada no uso manual: somente Pending pode ser cancelado.
        var secondCancelResponse = await client.PatchAsync(
            $"/api/orders/{created.Id}/cancel",
            content: null);
        Assert.Equal(HttpStatusCode.Conflict, secondCancelResponse.StatusCode);
        Assert.Equal(
            "application/problem+json",
            secondCancelResponse.Content.Headers.ContentType?.MediaType);
        using var conflictDocument = JsonDocument.Parse(
            await secondCancelResponse.Content.ReadAsStringAsync());
        Assert.Equal(
            "Only pending orders can be cancelled.",
            conflictDocument.RootElement.GetProperty("detail").GetString());
    }

    private async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = factory.CreateClient();
        var loginResponse = await client.PostAsJsonAsync("/auth/login", new
        {
            email = "dev@martech.com",
            password = "Senha@123"
        });

        loginResponse.EnsureSuccessStatusCode();
        var login = await loginResponse.Content.ReadFromJsonAsync<LoginContract>();
        Assert.NotNull(login);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login.AccessToken);

        return client;
    }

    private static async Task<OrderContract> ReadOrderAsync(HttpResponseMessage response)
    {
        var order = await response.Content.ReadFromJsonAsync<OrderContract>();
        return Assert.IsType<OrderContract>(order);
    }

    private sealed record LoginContract(string AccessToken, DateTime ExpiresAt);

    private sealed record OrderContract(Guid Id, string Status, decimal TotalAmount);
}
