using MediatR;

namespace OrderManagement.Application.Orders.CancelOrder;

/// <summary>Command que expressa a intenção de cancelar um pedido existente.</summary>
public sealed record CancelOrderCommand(Guid Id) : IRequest;
