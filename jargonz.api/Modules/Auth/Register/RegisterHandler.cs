using Microsoft.AspNetCore.Identity;
using jargonz.api.Common.Results;
using jargonz.api.Domain;

namespace jargonz.api.Modules.Auth.Register;

public static class RegisterHandler
{
    public static async Task<Result<RegisterResponse>> HandleAsync(
        RegisterCommand command,
        UserManager<AppUser> userManager)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is not null)
            return Result.Failure<RegisterResponse>(Error.Conflict(AuthErrors.EmailAlreadyExists, command.Email));

        var newUser = new AppUser
        {
            Id = Ulid.NewUlid(),
            Email = command.Email,
            NormalizedEmail = command.Email.ToUpper(),
            FullName = command.FullName,
            UserName = command.Email,
            TwoFactorEnabled = false
        };

        var result = await userManager.CreateAsync(newUser);
        if (!result.Succeeded)
            return Result.Failure<RegisterResponse>(Error.Validation("RegistrationFailed", "Registration failed"));

        var response = new RegisterResponse(
            newUser.Id,
            command.Email
        );

        return Result.Success(response);
    }
}
