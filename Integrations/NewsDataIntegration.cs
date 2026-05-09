using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace demomvcdata.Integrations;

public class NewsDataIntegration : INoticiasIntegration
{
    private readonly HttpClient _httpClient;
    private readonly IOptions<NewsDataOptions> _options;
    private readonly ILogger<NewsDataIntegration> _logger;

    public NewsDataIntegration(
        HttpClient httpClient,
        IOptions<NewsDataOptions> options,
        ILogger<NewsDataIntegration> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<IReadOnlyList<NoticiaItem>> GetNoticiasAsync(CancellationToken cancellationToken = default)
    {
        var settings = _options.Value;
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            _logger.LogWarning("No se configuró la API key de NewsData.");
            return Array.Empty<NoticiaItem>();
        }

        var requestUri = BuildRequestUri(settings);

        try
        {
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("NewsData respondió con estado {StatusCode}", response.StatusCode);
                return Array.Empty<NoticiaItem>();
            }

            var payload = await response.Content.ReadFromJsonAsync<NewsDataResponse>(cancellationToken: cancellationToken);
            return payload?.Results ?? new List<NoticiaItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar NewsData.");
            return Array.Empty<NoticiaItem>();
        }
    }

    private static string BuildRequestUri(NewsDataOptions settings)
    {
        return $"{settings.BaseUrl}?apikey={Uri.EscapeDataString(settings.ApiKey)}&country={Uri.EscapeDataString(settings.Country)}&language={Uri.EscapeDataString(settings.Language)}&q={Uri.EscapeDataString(settings.Query)}";
    }
}

public class NewsDataResponse
{
    [JsonPropertyName("results")]
    public List<NoticiaItem> Results { get; set; } = [];
}

public class NoticiaItem
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("source_id")]
    public string? SourceId { get; set; }

    [JsonPropertyName("pubDate")]
    public string? PublishedAt { get; set; }
}