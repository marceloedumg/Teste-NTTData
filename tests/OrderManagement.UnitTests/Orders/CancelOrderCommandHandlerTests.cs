using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.CancelOrder;
using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders;
using OrderManagement.UnitTests.Common;

namespace OrderManagement.UnitTests.Orders;

public sealed class CancelOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderIsPending_CancelsAndPersists()
    {
        var repository = new FakeOrderRepository();
        var order = CreateOrder();
        repository.Orders.Add(order);
        var handler = new CancelOrderCommandHandler(repository);

        await handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
        Assert.Equal(1, repository.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundException()
    {
        var handler = new CancelOrderCommandHandler(new FakeOrderRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new CancelOrderCommand(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenOrderIsNotPending_PropagatesDomainRule()
    {
        var repository = new FakeOrderRepository();
        var order = CreateOrder();
        order.Confirm();
        repository.Orders.Add(order);
        var handler = new CancelOrderCommandHandler(repository);

        await Assert.ThrowsAsync<DomainException>(() =>
            handler.Handle(new CancelOrderCommand(order.Id), CancellationToken.None));

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.Equal(0, repository.SaveChangesCalls);
    }

    private static Order CreateOrder() => Order.Create(
        Guid.NewGuid(),
        [new OrderItemDetails("Product", 1, 10m)],
        DateTime.UtcNow);
}
