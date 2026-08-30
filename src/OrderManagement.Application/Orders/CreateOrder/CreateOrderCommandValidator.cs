using FluentValidation;

namespace OrderManagement.Application.Orders.CreateOrder;

/// <summary>
/// Valida o contrato do command para fornecer erros detalhados antes da execução do caso de uso.
/// As mesmas invariantes continuam duplicadas de forma intencional no domínio como última barreira.
/// </summary>
public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.CustomerId)
            .NotEmpty();

        RuleFor(command => command.Items)
            .NotNull()
            .NotEmpty();

        RuleForEach(command => command.Items)
            .SetValidator(new CreateOrderItemValidator());
    }

    private sealed class CreateOrderItemValidator : AbstractValidator<CreateOrderItem>
    {
        public CreateOrderItemValidator()
        {
            RuleFor(item => item.ProductName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(item => item.Quantity)
                .GreaterThan(0);

            RuleFor(item => item.UnitPrice)
                .GreaterThan(0);
        }
    }
}
