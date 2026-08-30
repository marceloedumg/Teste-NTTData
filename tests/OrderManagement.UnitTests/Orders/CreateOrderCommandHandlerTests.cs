using OrderManagement.Application.Orders.CreateOrder;
using OrderManagement.Domain.Orders;
using OrderManagement.UnitTests.Common;

namespace OrderManagement.UnitTests.Orders;

public sealed class CreateOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_PersistsAndReturnsOrder()
    {
        var expectedTime = new DateTimeOffset(2026, 8, 30, 12, 30, 0, TimeSpan.Zero);
        var customerId = Guid.NewGuid();
        var repository = new FakeOrderRepository();
        var handler = new CreateOrderCommandHandler(
            repository,
            new FixedTimeProvider(expectedTime));

        var response = await handler.Handle(
            new CreateOrderCommand(
                customerId,
                [
                    new CreateOrderItem("Monitor", 2, 899.90m),
                    new CreateOrderItem("Cable", 3, 25m)
                ]),
            CancellationToken.None);

        var persistedOrder = Assert.Single(repository.Orders);
        Assert.Equal(persistedOrder.Id, response.Id);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(OrderStatus.Pending, response.Status);
        Assert.Equal(1_874.80m, response.TotalAmount);
        Assert.Equal(expectedTime.UtcDateTime, response.CreatedAt);
        Assert.Collection(
            response.Items,
            monitor =>
            {
                Assert.Equal("Monitor", monitor.ProductName);
                Assert.Equal(2, monitor.Quantity);
                Assert.Equal(899.90m, monitor.UnitPrice);
                Assert.Equal(1_799.80m, monitor.TotalAmount);
            },
            cable =>
            {
                Assert.Equal("Cable", cable.ProductName);
                Assert.Equal(3, cable.Quantity);
                Assert.Equal(25m, cable.UnitPrice);
                Assert.Equal(75m, cable.TotalAmount);
            });
        Assert.Equal(1, repository.SaveChangesCalls);
    }
}
