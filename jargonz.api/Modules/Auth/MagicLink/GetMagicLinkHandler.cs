using System.Security.Cryptography;
using jargonz.api.Common.EmailNotifications;
using jargonz.api.Common.Persistence;
using jargonz.api.Common.Results;
using jargonz.api.Domain;
using jargonz.api.Modules.Auth.Templates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace jargonz.api.Modules.Auth.MagicLink;

public static class GetMagicLinkHandler
{
    public static async Task<Result> HandleAsync(GetMagicLinkQuery query,
        DataContext context,
        UserManager<AppUser> userManager,
        EmailNotificationService emailService,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(query.Email);
        if (user == null)
            return Result.Success();

        await context.MagicLinkTokens.Where(x => x.Email == query.Email && x.UsedAt == null)
            .ExecuteUpdateAsync(x => x.SetProperty(g => g.UsedAt, DateTime.UtcNow),
                cancellationToken);

        var token = GenerateMagicLinkToken();

        context.MagicLinkTokens.Add(new MagicLinkToken
        {
            Email = query.Email,
            Token = token
        });
        await context.SaveChangesAsync(cancellationToken);

        var model = new MagicLinkEmailModel
        {
            Username = user.FullName ?? user.Email!,
            Code = token
        };

        await emailService.SendEmailAsync(
            query.Email,
            "Your Magic Link to Jargonz",
            "jargonz.api.Modules.Auth.Templates.MagicLink.cshtml",
            model,
            cancellationToken);

        return Result.Success();
    }

    private static string GenerateMagicLinkToken()
    {
        return RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
    }
}
