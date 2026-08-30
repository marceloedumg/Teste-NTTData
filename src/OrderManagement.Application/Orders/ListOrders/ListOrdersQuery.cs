using MediatR;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.ListOrders;

/// <summary>
/// Query paginada com padrões compatíveis com o contrato solicitado no teste.
/// </summary>
public sealed record ListOrdersQuery(int Page = 1, int PageSize = 10)
    : IRequest<PagedResult<OrderResponse>>;
