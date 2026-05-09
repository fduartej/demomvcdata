using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using demomvcdata.Models;
using System.Text.Json;
using demomvcdata.Services;

namespace demomvcdata.Controllers;

public class ZonasInsegurasController : Controller
{
    private readonly IZonasInsegurasService _zonasInsegurasService;
    private readonly ILogger<ZonasInsegurasController> _logger;

    public ZonasInsegurasController(IZonasInsegurasService zonasInsegurasService, ILogger<ZonasInsegurasController> logger)
    {
        _zonasInsegurasService = zonasInsegurasService;
        _logger = logger;
    }

    // GET: ZonasInseguras
    public async Task<IActionResult> Index(int? nivel)
    {
        ViewBag.NivelActual = nivel;

        var zonas = await _zonasInsegurasService.GetAllAsync(nivel);
        ViewData["ZonasCount"] = zonas.Count;

        return View(zonas);
    }

    // GET: ZonasInseguras/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var zonaInsegura = await _zonasInsegurasService.GetByIdAsync(id.Value);
        if (zonaInsegura == null)
        {
            return NotFound();
        }

        // Guardar la zona seleccionada en Session State
        HttpContext.Session.SetString("ZonaSeleccionada", JsonSerializer.Serialize(zonaInsegura));
        _logger.LogInformation("Zona '{NombreZona}' guardada en Session State", zonaInsegura.Nombre);

        return View(zonaInsegura);
    }

    // GET: ZonasInseguras/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ZonasInseguras/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Direccion,NivelPeligro,Descripcion,FechaRegistro,Activa")] ZonaInsegura zonaInsegura)
    {
        if (ModelState.IsValid)
        {
            await _zonasInsegurasService.CreateAsync(zonaInsegura);
            return RedirectToAction(nameof(Index));
        }
        return View(zonaInsegura);
    }

    // GET: ZonasInseguras/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var zonaInsegura = await _zonasInsegurasService.GetByIdAsync(id.Value);
        if (zonaInsegura == null)
        {
            return NotFound();
        }
        return View(zonaInsegura);
    }

    // POST: ZonasInseguras/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Direccion,NivelPeligro,Descripcion,FechaRegistro,Activa")] ZonaInsegura zonaInsegura)
    {
        if (id != zonaInsegura.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var updated = await _zonasInsegurasService.UpdateAsync(id, zonaInsegura);
                if (!updated)
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(zonaInsegura);
    }

    // GET: ZonasInseguras/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var zonaInsegura = await _zonasInsegurasService.GetByIdAsync(id.Value);
        if (zonaInsegura == null)
        {
            return NotFound();
        }

        return View(zonaInsegura);
    }

    // POST: ZonasInseguras/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _zonasInsegurasService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}
