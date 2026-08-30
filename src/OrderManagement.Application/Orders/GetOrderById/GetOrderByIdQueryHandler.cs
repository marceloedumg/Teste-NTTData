using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.GetOrderById;

/// <summary>
/// Consulta um pedido em modo somente leitura e o converte para o modelo de saída.
/// </summary>
public sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    public async Task<OrderResponse> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetReadOnlyAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        return order.ToResponse();
    }
}
