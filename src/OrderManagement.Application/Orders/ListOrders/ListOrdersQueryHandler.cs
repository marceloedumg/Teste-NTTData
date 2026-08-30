using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.ListOrders;

/// <summary>
/// Obtém somente a página solicitada e acrescenta metadados úteis ao consumidor.
/// </summary>
public sealed class ListOrdersQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<ListOrdersQuery, PagedResult<OrderResponse>>
{
    public async Task<PagedResult<OrderResponse>> Handle(
        ListOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetPageAsync(
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<OrderResponse>(
            result.Items.Select(order => order.ToResponse()).ToArray(),
            request.Page,
            request.PageSize,
            result.TotalCount);
    }
}
