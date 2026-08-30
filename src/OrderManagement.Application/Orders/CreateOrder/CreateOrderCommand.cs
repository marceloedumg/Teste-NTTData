using MediatR;

namespace OrderManagement.Application.Orders.CreateOrder;

/// <summary>
/// Command que expressa a intenção de criar e persistir um novo pedido.
/// </summary>
public sealed record CreateOrderCommand(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItem> Items) : IRequest<OrderResponse>;

/// <summary>
/// Dados de entrada de um item no caso de uso, separados do contrato HTTP e da entidade de domínio.
/// </summary>
public sealed record CreateOrderItem(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
