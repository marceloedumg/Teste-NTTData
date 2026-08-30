using FluentValidation;

namespace OrderManagement.Application.Authentication;

/// <summary>
/// Rejeita formatos inválidos antes de consultar o serviço de autenticação.
/// </summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(command => command.Password)
            .NotEmpty();
    }
}
