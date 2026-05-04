using System.IO.Hashing;
using System.Text;

namespace jargonz.api.Common.Extensions;

/// <summary>
///     Extension methods for string operations.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Computes a normalized, case-insensitive hash of the string.
    ///     The string is trimmed, lowercased, and hashed using SHA256.
    ///     Useful for duplicate detection where casing and whitespace should be ignored.
    /// </summary>
    public static string ToWordHash(this string value, Ulid userId, string contextSentence)
    {
        var normalized = string.Concat(value.Trim().ToLowerInvariant(), 
            userId.ToString(),
            contextSentence?.Trim().ToLowerInvariant());
        
        var bytes = XxHash64.Hash(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToBase64String(bytes).ToLowerInvariant();
    }
}
