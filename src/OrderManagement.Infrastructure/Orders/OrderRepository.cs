using Microsoft.EntityFrameworkCore;
using OrderManagement.Application.Abstractions;
using OrderManagement.Domain.Orders;
using OrderManagement.Infrastructure.Persistence;

namespace OrderManagement.Infrastructure.Orders;

/// <summary>
/// Implementa consultas específicas do agregado; detalhes do EF Core ficam confinados à Infrastructure.
/// </summary>
internal sealed class OrderRepository(OrdersDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<Order?> GetReadOnlyAsync(Guid id, CancellationToken cancellationToken) =>
        dbContext.Orders
            // Queries não precisam do custo de detecção de mudanças do EF Core.
            .AsNoTracking()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<OrderPage> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        // A paginação é executada no banco para não carregar toda a tabela em memória.
        var query = dbContext.Orders.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt)
            .ThenBy(order => order.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new OrderPage(items, totalCount);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
