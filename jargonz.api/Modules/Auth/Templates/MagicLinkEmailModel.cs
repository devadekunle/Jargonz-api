namespace jargonz.api.Modules.Auth.Templates;

public record MagicLinkEmailModel
{
    public required string Username { get; set; }
    public required string Code { get; set; }
}
