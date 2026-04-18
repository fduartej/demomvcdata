using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using demomvcdata.Models;

namespace demomvcdata.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Cookies()
    {
        // Leer las cookies del usuario
        ViewBag.UserEmail = Request.Cookies["UserEmail"] ?? "No disponible";
        ViewBag.UserId = Request.Cookies["UserId"] ?? "No disponible";
        ViewBag.UserName = Request.Cookies["UserName"] ?? "No disponible";
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
