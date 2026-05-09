namespace demomvcdata.Integrations;

public class MercadoPagoOptions
{
    public string AccessToken { get; set; } = string.Empty;
    public string CurrencyId { get; set; } = "PEN";
    public string ItemTitle { get; set; } = "Pago demomvcdata";
    public string? SuccessUrl { get; set; }
    public string? FailureUrl { get; set; }
    public string? PendingUrl { get; set; }
    public bool UseSandboxInitPoint { get; set; } = true;
}
