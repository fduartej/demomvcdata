using demomvcdata.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace demomvcdata.Controllers;

public class NoticiasController : Controller
{
    private readonly INoticiasIntegration _noticiasIntegration;

    public NoticiasController(INoticiasIntegration noticiasIntegration)
    {
        _noticiasIntegration = noticiasIntegration;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var noticias = await _noticiasIntegration.GetNoticiasAsync(cancellationToken);
        return View(noticias);
    }
}