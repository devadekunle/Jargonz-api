using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using jargonz.api.Common.Configuration;
using jargonz.api.Common.Results;

namespace jargonz.api.Common.Llm;

/// <summary>
///     Strongly-typed HTTP client for communicating with the DeepSeek API.
///     Uses the chat/completions endpoint with the selected model.
/// </summary>
public class DeepSeekApiClient
{
    private readonly HttpClient _httpClient;
    private readonly DeepSeekSettings _settings;
    private readonly ILogger<DeepSeekApiClient> _logger;

    public DeepSeekApiClient(
        HttpClient httpClient,
        DeepSeekSettings settings,
        ILogger<DeepSeekApiClient> logger)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
    }

    /// <summary>
    ///     Sends a chat completion request to DeepSeek and returns the response content.
    /// </summary>
    public async Task<Result<string>> GetChatCompletionAsync(
        string systemPrompt,
        string userMessage,
        CancellationToken ct = default)
    {
        try
        {
            var request = new ChatCompletionRequest
            {
                Model = _settings.Model,
                Messages =
                [
                    new ChatMessage { Role = "system", Content = systemPrompt },
                    new ChatMessage { Role = "user", Content = userMessage }
                ],
                Temperature = 0.3,
                MaxTokens = 1000
            };

            _logger.LogInformation("Sending request to DeepSeek API with model {Model}", _settings.Model);

            var response = await _httpClient.PostAsJsonAsync("chat/completions", request, ct);
            response.EnsureSuccessStatusCode();

            var completion = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: ct);

            if (completion?.Choices is not { Length: > 0 } ||
                string.IsNullOrEmpty(completion.Choices[0].Message?.Content))
            {
                _logger.LogWarning("DeepSeek returned empty response");
                return Result.Failure<string>(Error.Create("Llm.EmptyResponse", "LLM returned an empty response"));
            }

            var content = completion.Choices[0].Message.Content.Trim();

            // Handle code block wrapping (```json ... ```)
            if (content.StartsWith("```"))
            {
                var start = content.IndexOf('\n', 3) + 1;
                var end = content.LastIndexOf("```", StringComparison.Ordinal);
                content = content[start..end].Trim();
            }

            return Result.Success(content);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling DeepSeek API");
            return Result.Failure<string>(Error.Create("Llm.HttpError", $"Failed to call LLM API: {ex.Message}"));
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("DeepSeek API request timed out");
            return Result.Failure<string>(Error.Create("Llm.Timeout", "LLM API request timed out"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling DeepSeek API");
            return Result.Failure<string>(Error.Create("Llm.UnexpectedError", $"Unexpected LLM error: {ex.Message}"));
        }
    }
}

#region API DTOs

internal record ChatCompletionRequest
{
    [JsonPropertyName("model")]
    public required string Model { get; init; }

    [JsonPropertyName("messages")]
    public required ChatMessage[] Messages { get; init; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; init; } = 0.3;

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; init; } = 1000;

    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; init; }
}

internal record ResponseFormat
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "text";
}

internal record ChatMessage
{
    [JsonPropertyName("role")]
    public required string Role { get; init; }

    [JsonPropertyName("content")]
    public required string Content { get; init; }
}

internal record ChatCompletionResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("choices")]
    public Choice[]? Choices { get; init; }

    [JsonPropertyName("usage")]
    public Usage? Usage { get; init; }
}

internal record Choice
{
    [JsonPropertyName("index")]
    public int Index { get; init; }

    [JsonPropertyName("message")]
    public ChatMessage? Message { get; init; }

    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; init; } = string.Empty;
}

internal record Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}

#endregion
