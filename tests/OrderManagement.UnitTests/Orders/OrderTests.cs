using OrderManagement.Domain.Common;
using OrderManagement.Domain.Orders;

namespace OrderManagement.UnitTests.Orders;

public sealed class OrderTests
{
    private static readonly DateTime CreatedAt =
        new(2026, 8, 30, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithValidItems_CalculatesTotalInDomain()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            [
                new OrderItemDetails("Keyboard", 2, 150.50m),
                new OrderItemDetails("Mouse", 1, 99.90m)
            ],
            CreatedAt);

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(400.90m, order.TotalAmount);
        Assert.Equal(2, order.Items.Count);
        Assert.All(order.Items, item => Assert.Equal(order.Id, item.OrderId));
    }

    [Fact]
    public void Create_WithoutItems_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() =>
            Order.Create(Guid.NewGuid(), [], CreatedAt));

        Assert.Contains("at least one item", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithEmptyCustomerId_ThrowsDomainException()
    {
        var exception = Assert.Throws<DomainException>(() => Order.Create(
            Guid.Empty,
            [new OrderItemDetails("Product", 1, 10m)],
            CreatedAt));

        Assert.Contains("customer id", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNullItems_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Order.Create(Guid.NewGuid(), null!, CreatedAt));
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(1, -10)]
    public void Create_WithInvalidQuantityOrPrice_ThrowsDomainException(
        int quantity,
        decimal unitPrice)
    {
        Assert.Throws<DomainException>(() => Order.Create(
            Guid.NewGuid(),
            [new OrderItemDetails("Product", quantity, unitPrice)],
            CreatedAt));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyProductName_ThrowsDomainException(string productName)
    {
        Assert.Throws<DomainException>(() => Order.Create(
            Guid.NewGuid(),
            [new OrderItemDetails(productName, 1, 10m)],
            CreatedAt));
    }

    [Fact]
    public void Create_WithProductNameAboveLimit_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() => Order.Create(
            Guid.NewGuid(),
            [new OrderItemDetails(new string('A', 201), 1, 10m)],
            CreatedAt));
    }

    [Fact]
    public void Create_TrimsProductName()
    {
        var order = Order.Create(
            Guid.NewGuid(),
            [new OrderItemDetails("  Product  ", 1, 10m)],
            CreatedAt);

        Assert.Equal("Product", Assert.Single(order.Items).ProductName);
    }

    [Fact]
    public void Cancel_WhenOrderIsPending_ChangesStatusToCancelled()
    {
        var order = CreateOrder();

        order.Cancel();

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_WhenOrderIsNotPending_ThrowsDomainException()
    {
        var order = CreateOrder();
        order.Confirm();

        Assert.Throws<DomainException>(order.Cancel);
        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderIsPending_ChangesStatusToConfirmed()
    {
        var order = CreateOrder();

        order.Confirm();

        Assert.Equal(OrderStatus.Confirmed, order.Status);
    }

    [Fact]
    public void Confirm_WhenOrderIsNotPending_ThrowsDomainException()
    {
        var order = CreateOrder();
        order.Cancel();

        Assert.Throws<DomainException>(order.Confirm);
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    private static Order CreateOrder() => Order.Create(
        Guid.NewGuid(),
        [new OrderItemDetails("Product", 1, 10m)],
        CreatedAt);
}
