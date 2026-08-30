using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Orders;

/// <summary>
/// Raiz do agregado de pedidos e único ponto autorizado a proteger suas regras de negócio.
/// </summary>
public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    // Reservado ao EF Core. Os fluxos da aplicação devem usar Create para nunca produzir
    // um pedido sem cliente, sem item ou fora do estado inicial esperado.
    private Order()
    {
    }

    private Order(Guid id, Guid customerId, DateTime createdAt)
    {
        Id = id;
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Expõe os itens somente para leitura para impedir alterações que contornem o agregado.
    /// </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Obtém o total calculado a partir dos itens, conforme a regra de negócio.
    /// O valor não é armazenado para não existir uma segunda fonte de verdade.
    /// </summary>
    public decimal TotalAmount => _items.Sum(item => item.TotalAmount);

    /// <summary>
    /// Cria um pedido já consistente, pois todas as invariantes são verificadas antes do retorno.
    /// </summary>
    public static Order Create(
        Guid customerId,
        IEnumerable<OrderItemDetails> items,
        DateTime createdAt)
    {
        if (customerId == Guid.Empty)
        {
            throw new DomainException("Customer id is required.");
        }

        ArgumentNullException.ThrowIfNull(items);

        var itemDetails = items.ToArray();
        if (itemDetails.Length == 0)
        {
            throw new DomainException("An order must contain at least one item.");
        }

        // Datas em UTC tornam persistência, logs e integrações independentes do fuso do servidor.
        var order = new Order(Guid.NewGuid(), customerId, createdAt.ToUniversalTime());

        foreach (var details in itemDetails)
        {
            order._items.Add(OrderItem.Create(order.Id, details));
        }

        return order;
    }

    /// <summary>
    /// Cancela o pedido somente durante o estado pendente.
    /// A regra permanece no domínio para valer em qualquer interface de entrada.
    /// </summary>
    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can be cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }

    /// <summary>
    /// Confirma um pedido pendente e mantém a transição de estado centralizada no agregado.
    /// </summary>
    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
    }
}
