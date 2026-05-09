namespace demomvcdata.Integrations;

public class NewsDataOptions
{
    public string BaseUrl { get; set; } = "https://newsdata.io/api/1/news";
    public string ApiKey { get; set; } = string.Empty;
    public string Country { get; set; } = "pe";
    public string Language { get; set; } = "es";
    public string Query { get; set; } = "inseguridad";
}