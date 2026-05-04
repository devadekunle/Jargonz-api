using FluentValidation;
using jargonz.api.Common.Configuration;

namespace jargonz.api.Modules.Auth.MagicLink;

public record GetMagicLinkQuery(string Email);

public class GetMagicLinkQueryValidator : AbstractValidator<GetMagicLinkQuery>
{
    public GetMagicLinkQueryValidator(AllowedDomainSettings allowedDomain)
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address");

        RuleFor(x => x.Email)
            .Must(email => allowedDomain.Domain.Equals(email.Split('@')[1], StringComparison.OrdinalIgnoreCase))
            .When(x => allowedDomain.EnableValidation)
            .WithMessage("Your email is not valid for this system");
    }
}
