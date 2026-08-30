using OrderManagement.Application.Common;
using OrderManagement.Application.Orders.GetOrderById;
using OrderManagement.Domain.Orders;
using OrderManagement.UnitTests.Common;

namespace OrderManagement.UnitTests.Orders;

public sealed class GetOrderByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenOrderExists_ReturnsMappedOrder()
    {
        var customerId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 8, 30, 12, 0, 0, DateTimeKind.Utc);
        var repository = new FakeOrderRepository();
        var order = Order.Create(
            customerId,
            [new OrderItemDetails("Product", 2, 15m)],
            createdAt);
        repository.Orders.Add(order);
        var handler = new GetOrderByIdQueryHandler(repository);

        var response = await handler.Handle(
            new GetOrderByIdQuery(order.Id),
            CancellationToken.None);

        Assert.Equal(order.Id, response.Id);
        Assert.Equal(customerId, response.CustomerId);
        Assert.Equal(OrderStatus.Pending, response.Status);
        Assert.Equal(createdAt, response.CreatedAt);
        Assert.Equal(30m, response.TotalAmount);
        var item = Assert.Single(response.Items);
        Assert.Equal("Product", item.ProductName);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(15m, item.UnitPrice);
        Assert.Equal(30m, item.TotalAmount);
    }

    [Fact]
    public async Task Handle_WhenOrderDoesNotExist_ThrowsNotFoundException()
    {
        var handler = new GetOrderByIdQueryHandler(new FakeOrderRepository());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
