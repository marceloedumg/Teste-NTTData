using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders;

/// <summary>
/// Modelo de saída estável da Application, evitando expor entidades persistidas diretamente na API.
/// </summary>
public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    OrderStatus Status,
    DateTime CreatedAt,
    decimal TotalAmount,
    IReadOnlyCollection<OrderItemResponse> Items);

/// <summary>Representa um item no modelo de saída de um pedido.</summary>
public sealed record OrderItemResponse(
    Guid Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount);
