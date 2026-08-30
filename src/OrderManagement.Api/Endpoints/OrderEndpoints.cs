using MediatR;
using OrderManagement.Api.Contracts.Orders;
using OrderManagement.Application.Common;
using OrderManagement.Application.Orders;
using OrderManagement.Application.Orders.CancelOrder;
using OrderManagement.Application.Orders.CreateOrder;
using OrderManagement.Application.Orders.GetOrderById;
using OrderManagement.Application.Orders.ListOrders;

namespace OrderManagement.Api.Endpoints;

/// <summary>
/// Define os contratos HTTP de pedidos sem conter decisões de negócio.
/// </summary>
internal static class OrderEndpoints
{
    internal static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Autorização no grupo evita esquecer a proteção ao adicionar um novo endpoint de pedidos.
        var group = endpoints.MapGroup("/api/orders")
            .RequireAuthorization()
            .WithTags("Orders");

        group.MapPost("/", CreateOrderAsync)
            .WithName("CreateOrder")
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/", ListOrdersAsync)
            .WithName("ListOrders")
            .Produces<PagedResult<OrderResponse>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{id:guid}", GetOrderByIdAsync)
            .WithName("GetOrderById")
            .Produces<OrderResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPatch("/{id:guid}/cancel", CancelOrderAsync)
            .WithName("CancelOrder")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }

    private static async Task<IResult> CreateOrderAsync(
        CreateOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        // Null vira coleção vazia para que o ValidationBehavior produza 400 em vez de NullReferenceException.
        var items = request.Items?
            .Select(item => new CreateOrderItem(
                item.ProductName,
                item.Quantity,
                item.UnitPrice))
            .ToArray() ?? [];

        var response = await sender.Send(
            new CreateOrderCommand(request.CustomerId, items),
            cancellationToken);

        return Results.Created($"/api/orders/{response.Id}", response);
    }

    private static async Task<IResult> ListOrdersAsync(
        ISender sender,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var response = await sender.Send(
            new ListOrdersQuery(page, pageSize),
            cancellationToken);

        return Results.Ok(response);
    }

    private static async Task<IResult> GetOrderByIdAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Results.Ok(response);
    }

    private static async Task<IResult> CancelOrderAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CancelOrderCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
