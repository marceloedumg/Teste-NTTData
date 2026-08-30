using OrderManagement.Application.Orders.ListOrders;
using OrderManagement.Domain.Orders;
using OrderManagement.UnitTests.Common;

namespace OrderManagement.UnitTests.Orders;

public sealed class ListOrdersQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenThereAreNoOrders_ReturnsEmptyPageWithZeroTotalPages()
    {
        var handler = new ListOrdersQueryHandler(new FakeOrderRepository());

        var response = await handler.Handle(
            new ListOrdersQuery(Page: 1, PageSize: 10),
            CancellationToken.None);

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);
    }

    [Fact]
    public async Task Handle_ReturnsRequestedPageAndMetadata()
    {
        var repository = new FakeOrderRepository();
        for (var index = 0; index < 3; index++)
        {
            repository.Orders.Add(Order.Create(
                Guid.NewGuid(),
                [new OrderItemDetails($"Product {index}", 1, 10m)],
                DateTime.UtcNow.AddMinutes(index)));
        }

        var handler = new ListOrdersQueryHandler(repository);

        var response = await handler.Handle(
            new ListOrdersQuery(Page: 2, PageSize: 2),
            CancellationToken.None);

        Assert.Single(response.Items);
        Assert.Equal(2, response.Page);
        Assert.Equal(2, response.PageSize);
        Assert.Equal(3, response.TotalCount);
        Assert.Equal(2, response.TotalPages);
    }
}
