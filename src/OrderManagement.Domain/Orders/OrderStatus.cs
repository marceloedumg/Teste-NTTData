namespace OrderManagement.Domain.Orders;

/// <summary>
/// Representa os estados permitidos para o ciclo de vida de um pedido.
/// Os valores explícitos evitam alterações acidentais caso a ordem dos membros seja modificada.
/// </summary>
public enum OrderStatus
{
    /// <summary>Estado inicial, no qual o pedido ainda pode ser confirmado ou cancelado.</summary>
    Pending = 1,

    /// <summary>Indica que o pedido foi confirmado e não pode mais ser cancelado.</summary>
    Confirmed = 2,

    /// <summary>Indica que o pedido foi cancelado enquanto ainda estava pendente.</summary>
    Cancelled = 3
}
