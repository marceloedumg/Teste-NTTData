using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders.CreateOrder;

/// <summary>
/// Coordena criação e persistência; as regras permanecem no agregado <see cref="Order"/>.
/// </summary>
public sealed class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    TimeProvider timeProvider)
    : IRequestHandler<CreateOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var itemDetails = request.Items.Select(item =>
            new OrderItemDetails(item.ProductName, item.Quantity, item.UnitPrice));

        // O relógio injetável torna CreatedAt determinístico em testes sem deslocar a regra para o handler.
        var order = Order.Create(
            request.CustomerId,
            itemDetails,
            timeProvider.GetUtcNow().UtcDateTime);

        await orderRepository.AddAsync(order, cancellationToken);
        await orderRepository.SaveChangesAsync(cancellationToken);

        return order.ToResponse();
    }
}
