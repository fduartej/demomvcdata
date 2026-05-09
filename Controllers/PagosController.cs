using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using demomvcdata.Integrations;
using demomvcdata.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace demomvcdata.Controllers;

public class PagosController : Controller
{
    private const string MercadoPagoApiBaseUrl = "https://api.mercadopago.com";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PagosController> _logger;
    private readonly MercadoPagoOptions _mercadoPagoOptions;

    public PagosController(
        IHttpClientFactory httpClientFactory,
        ILogger<PagosController> logger,
        IOptions<MercadoPagoOptions> mercadoPagoOptions)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _mercadoPagoOptions = mercadoPagoOptions.Value;
    }

    [HttpGet]
    public IActionResult MercadoPago()
    {
        return View(new MercadoPagoCheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MercadoPago(MercadoPagoCheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(_mercadoPagoOptions.AccessToken))
        {
            ModelState.AddModelError(string.Empty, "Falta configurar MercadoPago:AccessToken en User Secrets o appsettings.");
            return View(model);
        }

        var successUrl = ResolveBackUrl(_mercadoPagoOptions.SuccessUrl);

        if (string.IsNullOrWhiteSpace(successUrl))
        {
            ModelState.AddModelError(string.Empty, "No se pudo construir la URL de retorno. Configura MercadoPago:SuccessUrl con una URL absoluta (https://...).");
            return View(model);
        }

        var failureUrl = ResolveBackUrl(_mercadoPagoOptions.FailureUrl) ?? successUrl;
        var pendingUrl = ResolveBackUrl(_mercadoPagoOptions.PendingUrl) ?? successUrl;

        var payload = new
        {
            items = new[]
            {
                new
                {
                    title = string.IsNullOrWhiteSpace(model.Descripcion) ? _mercadoPagoOptions.ItemTitle : model.Descripcion,
                    quantity = 1,
                    unit_price = model.Monto,
                    currency_id = _mercadoPagoOptions.CurrencyId
                }
            },
            payer = new
            {
                identification = new
                {
                    type = "DNI",
                    number = model.Dni
                }
            },
            back_urls = new
            {
                success = successUrl,
                failure = failureUrl,
                pending = pendingUrl
            },
            auto_return = "approved"
        };

        var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(MercadoPagoApiBaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mercadoPagoOptions.AccessToken);

        var requestContent = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        var response = await httpClient.PostAsync("/checkout/preferences", requestContent);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Error creando preferencia Mercado Pago: {StatusCode} - {Body}", response.StatusCode, responseBody);
            ModelState.AddModelError(string.Empty, "No se pudo iniciar el pago en Mercado Pago. Revisa credenciales y vuelve a intentar.");
            return View(model);
        }

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        string? redirectUrl = null;

        if (_mercadoPagoOptions.UseSandboxInitPoint &&
            root.TryGetProperty("sandbox_init_point", out var sandboxInitPointElement))
        {
            redirectUrl = sandboxInitPointElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(redirectUrl) &&
            root.TryGetProperty("init_point", out var initPointElement))
        {
            redirectUrl = initPointElement.GetString();
        }

        if (string.IsNullOrWhiteSpace(redirectUrl))
        {
            ModelState.AddModelError(string.Empty, "Mercado Pago no devolvio una URL de pago valida.");
            return View(model);
        }

        return Redirect(redirectUrl);
    }

    private string? ResolveBackUrl(string? configuredUrl)
    {
        if (Uri.TryCreate(configuredUrl, UriKind.Absolute, out var configuredUri) &&
            (configuredUri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ||
             configuredUri.Scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)))
        {
            return configuredUri.ToString();
        }

        if (Request.Host.HasValue)
        {
            var localPath = Url.Action("MercadoPago", "Pagos") ?? "/Pagos/MercadoPago";
            return $"{Request.Scheme}://{Request.Host}{localPath}";
        }

        return null;
    }
}
