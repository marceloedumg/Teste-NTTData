namespace OrderManagement.Domain.Orders;

/// <summary>
/// Transporta os dados necessários para o agregado criar um item.
/// O tipo vive no domínio para que a criação não dependa de contratos da API ou da Application.
/// </summary>
public sealed record OrderItemDetails(
    string ProductName,
    int Quantity,
    decimal UnitPrice);
