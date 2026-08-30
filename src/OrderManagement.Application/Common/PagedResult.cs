namespace OrderManagement.Application.Common;

/// <summary>
/// Padroniza itens e metadados de paginação para que clientes possam navegar sem inferências.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    /// <summary>Calcula a quantidade de páginas sem persistir informação derivada.</summary>
    public int TotalPages => TotalCount == 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
