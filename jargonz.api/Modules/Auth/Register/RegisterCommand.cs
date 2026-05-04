using FluentValidation;
using jargonz.api.Common.Configuration;

namespace jargonz.api.Modules.Auth.Register;

/// <summary>
///     Command for user registration
/// </summary>
public record RegisterCommand(string Email, string FullName);

/// <summary>
///     Response for successful registration
/// </summary>
public record RegisterResponse(Ulid UserId, string Email);

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(AllowedDomainSettings allowedDomain)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MinimumLength(2).WithMessage("Full name must be at least 2 characters long");
    }
}
