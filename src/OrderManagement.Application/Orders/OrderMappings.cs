using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Orders;

/// <summary>
/// Mantém a transformação de domínio para resposta em um único lugar, sem duplicá-la nos handlers.
/// </summary>
internal static class OrderMappings
{
    internal static OrderResponse ToResponse(this Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Status,
            order.CreatedAt,
            order.TotalAmount,
            order.Items
                .Select(item => new OrderItemResponse(
                    item.Id,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalAmount))
                .ToArray());
}
