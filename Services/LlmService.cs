using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ClipboardAI.Services;

public class LlmService
{
    private readonly HttpClient _httpClient;
    private readonly ConfigManager _config;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LlmService(ConfigManager config)
    {
        _config = config;
        _httpClient = new HttpClient();
    }

    public async Task<string?> ProcessTextAsync(string text, CancellationToken ct = default)
    {
        var cfg = _config.Data;
        var systemPrompt = _config.GetCurrentSystemPrompt();
        if (string.IsNullOrWhiteSpace(systemPrompt))
            return null;

        var requestBody = new
        {
            model = cfg.Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = text }
            },
            max_tokens = cfg.MaxTokens,
            temperature = cfg.Temperature
        };

        var json = JsonSerializer.Serialize(requestBody, JsonOpts);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{cfg.ApiBase.TrimEnd('/')}/chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cfg.ApiKey);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(responseJson);
        var result = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return result?.Trim();
    }
}
