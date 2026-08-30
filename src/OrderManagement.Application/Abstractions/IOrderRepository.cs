using OrderManagement.Domain.Orders;

namespace OrderManagement.Application.Abstractions;

/// <summary>
/// Define somente as operações de persistência exigidas pelos casos de uso de pedidos.
/// Um contrato específico deixa as intenções explícitas e evita um repositório genérico sem propósito.
/// </summary>
public interface IOrderRepository
{
    /// <summary>Adiciona uma nova raiz de agregado à unidade de trabalho atual.</summary>
    Task AddAsync(Order order, CancellationToken cancellationToken);

    /// <summary>Carrega um pedido com tracking porque commands podem alterar seu estado.</summary>
    Task<Order?> GetAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Carrega um pedido sem tracking para consultas que não realizarão alterações.</summary>
    Task<Order?> GetReadOnlyAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Retorna somente a página solicitada e a contagem necessária para os metadados.</summary>
    Task<OrderPage> GetPageAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Confirma atomicamente as mudanças coordenadas pelo handler.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Resultado interno de paginação usado entre Infrastructure e Application.
/// </summary>
public sealed record OrderPage(IReadOnlyCollection<Order> Items, int TotalCount);
