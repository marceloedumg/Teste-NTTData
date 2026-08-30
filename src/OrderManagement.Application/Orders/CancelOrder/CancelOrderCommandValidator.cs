using FluentValidation;

namespace OrderManagement.Application.Orders.CancelOrder;

/// <summary>Impede que um identificador vazio chegue à persistência.</summary>
public sealed class CancelOrderCommandValidator : AbstractValidator<CancelOrderCommand>
{
    public CancelOrderCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
    }
}
