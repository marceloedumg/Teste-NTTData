using FluentValidation;
using OrderManagement.Application.Authentication;
using OrderManagement.Application.Orders.CancelOrder;
using OrderManagement.Application.Orders.CreateOrder;
using OrderManagement.Application.Orders.GetOrderById;
using OrderManagement.Application.Orders.ListOrders;

namespace OrderManagement.UnitTests.Validation;

public sealed class ApplicationValidatorsTests
{
    [Theory]
    [InlineData("", "Senha@123", "Email")]
    [InlineData("invalid-email", "Senha@123", "Email")]
    [InlineData("dev@martech.com", "", "Password")]
    public void Login_WithInvalidInput_ReturnsExpectedFailure(
        string email,
        string password,
        string propertyName)
    {
        var result = new LoginCommandValidator().Validate(new LoginCommand(email, password));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == propertyName);
    }

    [Fact]
    public void CreateOrder_WithInvalidItem_ReturnsAllRelevantFailures()
    {
        var command = new CreateOrderCommand(
            Guid.Empty,
            [new CreateOrderItem(new string('A', 201), 0, 0m)]);

        var result = new CreateOrderCommandValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "CustomerId");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].ProductName");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].Quantity");
        Assert.Contains(result.Errors, error => error.PropertyName == "Items[0].UnitPrice");
    }

    [Fact]
    public void CreateOrder_WithoutItems_ReturnsItemsFailure()
    {
        var result = new CreateOrderCommandValidator().Validate(
            new CreateOrderCommand(Guid.NewGuid(), []));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Items");
    }

    [Fact]
    public void CommandsAndQueries_WithEmptyIds_AreInvalid()
    {
        var cancelResult = new CancelOrderCommandValidator().Validate(
            new CancelOrderCommand(Guid.Empty));
        var getResult = new GetOrderByIdQueryValidator().Validate(
            new GetOrderByIdQuery(Guid.Empty));

        Assert.False(cancelResult.IsValid);
        Assert.Contains(cancelResult.Errors, error => error.PropertyName == "Id");
        Assert.False(getResult.IsValid);
        Assert.Contains(getResult.Errors, error => error.PropertyName == "Id");
    }

    [Theory]
    [InlineData(0, 10, "Page")]
    [InlineData(1, 0, "PageSize")]
    [InlineData(1, 101, "PageSize")]
    public void ListOrders_WithInvalidPagination_ReturnsExpectedFailure(
        int page,
        int pageSize,
        string propertyName)
    {
        var result = new ListOrdersQueryValidator().Validate(
            new ListOrdersQuery(page, pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == propertyName);
    }

    [Fact]
    public void Validators_WithValidInput_AcceptRequests()
    {
        Assert.True(new LoginCommandValidator()
            .Validate(new LoginCommand("dev@martech.com", "Senha@123")).IsValid);
        Assert.True(new CreateOrderCommandValidator()
            .Validate(new CreateOrderCommand(
                Guid.NewGuid(),
                [new CreateOrderItem("Product", 1, 10m)])).IsValid);
        Assert.True(new CancelOrderCommandValidator()
            .Validate(new CancelOrderCommand(Guid.NewGuid())).IsValid);
        Assert.True(new GetOrderByIdQueryValidator()
            .Validate(new GetOrderByIdQuery(Guid.NewGuid())).IsValid);
        Assert.True(new ListOrdersQueryValidator()
            .Validate(new ListOrdersQuery()).IsValid);
    }
}
