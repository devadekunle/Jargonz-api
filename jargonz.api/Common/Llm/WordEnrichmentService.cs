using System.Text.Json;
using System.Text.Json.Serialization;

namespace jargonz.api.Common.Llm;

/// <summary>
///     Result from LLM word enrichment.
/// </summary>
public record WordEnrichmentResult(
    string Definition,
    string Phonetic,
    string PartOfSpeech,
    string Etymology,
    string ExampleSentence);

/// <summary>
///     Service that enriches a word entry by calling the LLM (DeepSeek)
///     to populate definition, phonetic, part of speech, etymology, and example sentence.
/// </summary>
public class WordEnrichmentService
{
    private readonly DeepSeekApiClient _deepSeekClient;
    private readonly ILogger<WordEnrichmentService> _logger;

    public WordEnrichmentService(
        DeepSeekApiClient deepSeekClient,
        ILogger<WordEnrichmentService> logger)
    {
        _deepSeekClient = deepSeekClient;
        _logger = logger;
    }

    /// <summary>
    ///     Enriches a word with definition, phonetic, part of speech, etymology, and example sentence.
    ///     Returns null if enrichment fails (caller should handle gracefully).
    /// </summary>
    public async Task<WordEnrichmentResult?> EnrichWordAsync(
        string word,
        string? contextSentence,
        CancellationToken ct = default)
    {
        var contextClause = string.IsNullOrEmpty(contextSentence)
            ? string.Empty
            : $" found in the context: \"{contextSentence}\"";

        var systemPrompt = """
            You are a lexicographer and English language expert. Your task is to provide accurate,
            concise linguistic information for English words. You must ALWAYS respond with valid JSON
            only — no markdown, no code fences, no explanations, no extra text.

            For the given word, provide:
            1. definition: A concise definition (1-2 sentences) in plain English
            2. phonetic: IPA phonetic transcription (e.g., /ˈɛksəmpəl/)
            3. partOfSpeech: The part of speech (e.g., noun, verb, adjective, adverb, etc.)
            4. etymology: A brief etymology/origin of the word
            5. exampleSentence: An example sentence showing proper usage of the word

            RULES:
            - Respond ONLY with a JSON object. No other text.
            - Use exactly these keys: definition, phonetic, partOfSpeech, etymology, exampleSentence
            - All values must be strings.
            - If you cannot determine a field, use an empty string.
            - Do NOT wrap the JSON in code fences or markdown.
            """;

        var userMessage = $"Provide linguistic information for the word: \"{word}\"{contextClause}";

        var result = await _deepSeekClient.GetChatCompletionAsync(systemPrompt, userMessage, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("Failed to enrich word '{Word}': {Error}", word, result.Error.Message);
            return null;
        }

        try
        {
            var enrichment = JsonSerializer.Deserialize<WordEnrichmentJson>(result.Value,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (enrichment is null)
            {
                _logger.LogWarning("Failed to deserialize LLM response for word '{Word}'", word);
                return null;
            }

            _logger.LogInformation("Successfully enriched word '{Word}'", word);

            return new WordEnrichmentResult(
                enrichment.Definition ?? string.Empty,
                enrichment.Phonetic ?? string.Empty,
                enrichment.PartOfSpeech ?? string.Empty,
                enrichment.Etymology ?? string.Empty,
                enrichment.ExampleSentence ?? string.Empty);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse LLM JSON response for word '{Word}'. Response: {Response}",
                word, result.Value);
            return null;
        }
    }

    private class WordEnrichmentJson
    {
        [JsonPropertyName("definition")]
        public string? Definition { get; init; }

        [JsonPropertyName("phonetic")]
        public string? Phonetic { get; init; }

        [JsonPropertyName("partOfSpeech")]
        public string? PartOfSpeech { get; init; }

        [JsonPropertyName("etymology")]
        public string? Etymology { get; init; }

        [JsonPropertyName("exampleSentence")]
        public string? ExampleSentence { get; init; }
    }
}
