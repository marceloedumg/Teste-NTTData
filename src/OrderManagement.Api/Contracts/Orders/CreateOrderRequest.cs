namespace OrderManagement.Api.Contracts.Orders;

/// <summary>
/// Contrato HTTP de criação; aceita itens nulos para que o pipeline devolva um erro de validação controlado.
/// </summary>
public sealed record CreateOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemRequest>? Items);

/// <summary>Representa os dados enviados para cada item do novo pedido.</summary>
public sealed record CreateOrderItemRequest(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
