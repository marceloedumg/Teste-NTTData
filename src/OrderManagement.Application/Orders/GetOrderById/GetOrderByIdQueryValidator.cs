using FluentValidation;

namespace OrderManagement.Application.Orders.GetOrderById;

/// <summary>Impede uma consulta ao banco com identificador vazio.</summary>
public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(query => query.Id).NotEmpty();
    }
}
