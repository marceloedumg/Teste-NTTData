using FluentValidation;

namespace OrderManagement.Application.Orders.ListOrders;

/// <summary>
/// Protege o banco contra páginas inválidas e limita o volume retornado por requisição.
/// </summary>
public sealed class ListOrdersQueryValidator : AbstractValidator<ListOrdersQuery>
{
    public ListOrdersQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100);
    }
}
