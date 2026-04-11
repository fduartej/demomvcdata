using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using demomvcdata.Data;
using demomvcdata.Models;

namespace demomvcdata.Controllers;

public class ZonasInsegurasController : Controller
{
    private readonly ApplicationDbContext _context;

    public ZonasInsegurasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: ZonasInseguras
    public async Task<IActionResult> Index()
    {
        return View(await _context.ZonasInseguras.ToListAsync());
    }

    // GET: ZonasInseguras/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var zonaInsegura = await _context.ZonasInseguras
            .FirstOrDefaultAsync(m => m.Id == id);
        if (zonaInsegura == null)
        {
            return NotFound();
        }

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
            _context.Add(zonaInsegura);
            await _context.SaveChangesAsync();
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

        var zonaInsegura = await _context.ZonasInseguras.FindAsync(id);
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
                _context.Update(zonaInsegura);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ZonaInseguraExists(zonaInsegura.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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

        var zonaInsegura = await _context.ZonasInseguras
            .FirstOrDefaultAsync(m => m.Id == id);
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
        var zonaInsegura = await _context.ZonasInseguras.FindAsync(id);
        if (zonaInsegura != null)
        {
            _context.ZonasInseguras.Remove(zonaInsegura);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ZonaInseguraExists(int id)
    {
        return _context.ZonasInseguras.Any(e => e.Id == id);
    }
}
