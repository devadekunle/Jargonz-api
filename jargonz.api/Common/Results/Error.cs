namespace jargonz.api.Common.Results;

/// <summary>
///     Represents an error with a code and message
/// </summary>
public sealed record Error
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified value is null");

    private Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }
    public string Message { get; }

    public static Error Create(string code, string message)
    {
        return new Error(code, message);
    }

    public static Error NotFound(string entityName, string identifier)
    {
        return new Error($"{entityName}.NotFound", $"{entityName} with identifier '{identifier}' was not found");
    }

    public static Error Validation(string code, string message)
    {
        return new Error($"Validation.{code}", message);
    }

    public static Error Conflict(string code, string message)
    {
        return new Error($"Conflict.{code}", message);
    }

    public static Error Unauthorized(string message = "Unauthorized access")
    {
        return new Error("Error.Unauthorized", message);
    }

    public static Error Forbidden(string message = "Forbidden access")
    {
        return new Error("Error.Forbidden", message);
    }

    public static implicit operator string(Error error)
    {
        return error.Code;
    }
}
