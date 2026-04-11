using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using demomvcdata.Data;
using demomvcdata.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace demomvcdata.Controllers;

public class ZonasInsegurasController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ZonasInsegurasController> _logger;


    private const string CacheKeyPrefix = "zonas-inseguras:index";

    public ZonasInsegurasController(ApplicationDbContext context, IDistributedCache cache, ILogger<ZonasInsegurasController> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    // GET: ZonasInseguras
    public async Task<IActionResult> Index(int? nivel)
    {
        ViewBag.NivelActual = nivel;

        var cacheKey = BuildCacheKey(nivel);
        var zonasDesdeCache = await TryGetIndexFromCacheAsync(cacheKey);
        if (zonasDesdeCache != null)
        {
            _logger.LogInformation("Zonas inseguras obtenidas desde la caché.");
            return View(zonasDesdeCache);
        }

        var zonas = _context.ZonasInseguras.AsQueryable();

        if (nivel.HasValue)
        {
            zonas = zonas.Where(z => z.NivelPeligro == nivel.Value);
        }

        var resultado = await zonas.ToListAsync();
        await TrySetIndexInCacheAsync(cacheKey, resultado);

        return View(resultado);
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
            await InvalidateIndexCacheAsync();
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
                await InvalidateIndexCacheAsync();
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
        await InvalidateIndexCacheAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ZonaInseguraExists(int id)
    {
        return _context.ZonasInseguras.Any(e => e.Id == id);
    }

    private static string BuildCacheKey(int? nivel)
    {
        return nivel.HasValue ? $"{CacheKeyPrefix}:nivel:{nivel.Value}" : $"{CacheKeyPrefix}:all";
    }

    private async Task InvalidateIndexCacheAsync()
    {
        await TryRemoveCacheKeyAsync(BuildCacheKey(null));

        for (var nivel = 1; nivel <= 5; nivel++)
        {
            await TryRemoveCacheKeyAsync(BuildCacheKey(nivel));
        }
    }

    private async Task<List<ZonaInsegura>?> TryGetIndexFromCacheAsync(string cacheKey)
    {
        try
        {
            var zonasCached = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrWhiteSpace(zonasCached))
            {
                return null;
            }

            return JsonSerializer.Deserialize<List<ZonaInsegura>>(zonasCached);
        }
        catch (RedisConnectionException)
        {
            return null;
        }
        catch (RedisTimeoutException)
        {
            return null;
        }
    }

    private async Task TrySetIndexInCacheAsync(string cacheKey, List<ZonaInsegura> zonas)
    {
        try
        {
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(zonas),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }
        catch (RedisConnectionException)
        {
        }
        catch (RedisTimeoutException)
        {
        }
    }

    private async Task TryRemoveCacheKeyAsync(string cacheKey)
    {
        try
        {
            await _cache.RemoveAsync(cacheKey);
        }
        catch (RedisConnectionException)
        {
        }
        catch (RedisTimeoutException)
        {
        }
    }
}
