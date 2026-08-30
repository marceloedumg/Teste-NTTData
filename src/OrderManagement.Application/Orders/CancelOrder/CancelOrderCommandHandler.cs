using MediatR;
using OrderManagement.Application.Abstractions;
using OrderManagement.Application.Common;

namespace OrderManagement.Application.Orders.CancelOrder;

/// <summary>
/// Carrega o agregado e delega a ele a decisão sobre a permissão de cancelamento.
/// </summary>
public sealed class CancelOrderCommandHandler(IOrderRepository orderRepository)
    : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        order.Cancel();

        await orderRepository.SaveChangesAsync(cancellationToken);
    }
}
