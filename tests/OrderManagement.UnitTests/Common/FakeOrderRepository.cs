using OrderManagement.Application.Abstractions;
using OrderManagement.Domain.Orders;

namespace OrderManagement.UnitTests.Common;

/// <summary>
/// Fake manual que torna os testes de handlers rápidos e deixa explícito quando houve persistência.
/// </summary>
internal sealed class FakeOrderRepository : IOrderRepository
{
    internal List<Order> Orders { get; } = [];

    internal int SaveChangesCalls { get; private set; }

    public Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Orders.SingleOrDefault(order => order.Id == id));

    public Task<Order?> GetReadOnlyAsync(Guid id, CancellationToken cancellationToken) =>
        Task.FromResult(Orders.SingleOrDefault(order => order.Id == id));

    public Task<OrderPage> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var items = Orders
            .OrderByDescending(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult(new OrderPage(items, Orders.Count));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCalls++;
        return Task.CompletedTask;
    }
}
