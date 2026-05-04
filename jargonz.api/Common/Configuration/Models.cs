namespace jargonz.api.Common.Configuration;

public record AllowedDomainSettings(string Domain, bool EnableValidation)
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(Domain))
            throw new ArgumentException("Domain cannot be empty");
    }
}

public record JwtSettings(string Issuer, string Audience, string Key, TimeSpan Expiration)
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(Issuer))
            throw new ArgumentException("Issuer cannot be empty");
        if (string.IsNullOrEmpty(Audience))
            throw new ArgumentException("Audience cannot be empty");
        if (string.IsNullOrEmpty(Key))
            throw new ArgumentException("Key cannot be empty");
    }
}

public record DeepSeekSettings(string ApiKey, string Model, string BaseUrl)
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentException("DeepSeek ApiKey cannot be empty");
        if (string.IsNullOrEmpty(Model))
            throw new ArgumentException("DeepSeek Model cannot be empty");
        if (string.IsNullOrEmpty(BaseUrl))
            throw new ArgumentException("DeepSeek BaseUrl cannot be empty");
    }
}

public record EmailSettings(
    string ApiKey,
    string FromEmail,
    string FromName)
{
    public void Validate()
    {
        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentException("ApiKey cannot be empty");
        if (string.IsNullOrEmpty(FromEmail))
            throw new ArgumentException("FromEmail cannot be empty");
        if (string.IsNullOrEmpty(FromName))
            throw new ArgumentException("FromName cannot be empty");
    }
}
