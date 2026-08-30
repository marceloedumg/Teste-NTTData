using MediatR;

namespace OrderManagement.Application.Orders.GetOrderById;

/// <summary>Query de leitura de um único pedido, separada dos commands que alteram estado.</summary>
public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderResponse>;
