using OrderManagement.Domain.Common;

namespace OrderManagement.Domain.Orders;

/// <summary>
/// Representa uma linha pertencente a um pedido.
/// A criação é controlada pelo agregado <see cref="Order"/> para preservar vínculo e invariantes.
/// </summary>
public sealed class OrderItem
{
    // O EF Core precisa de um construtor sem parâmetros para materializar a entidade.
    // Ele permanece privado para impedir a criação de itens inválidos pelo código de negócio.
    private OrderItem()
    {
    }

    private OrderItem(
        Guid id,
        Guid orderId,
        string productName,
        int quantity,
        decimal unitPrice)
    {
        Id = id;
        OrderId = orderId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Obtém o subtotal calculado no domínio, mantendo a regra monetária junto aos dados que a originam.
    /// </summary>
    public decimal TotalAmount => Quantity * UnitPrice;

    /// <summary>
    /// Cria um item válido e associado ao identificador da raiz do agregado.
    /// </summary>
    internal static OrderItem Create(Guid orderId, OrderItemDetails details)
    {
        if (string.IsNullOrWhiteSpace(details.ProductName))
        {
            throw new DomainException("Product name is required.");
        }

        if (details.ProductName.Trim().Length > 200)
        {
            throw new DomainException("Product name must contain at most 200 characters.");
        }

        if (details.Quantity <= 0)
        {
            throw new DomainException("Item quantity must be greater than zero.");
        }

        if (details.UnitPrice <= 0)
        {
            throw new DomainException("Item unit price must be greater than zero.");
        }

        return new OrderItem(
            Guid.NewGuid(),
            orderId,
            details.ProductName.Trim(),
            details.Quantity,
            details.UnitPrice);
    }
}
