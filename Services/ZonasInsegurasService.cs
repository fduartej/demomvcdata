using demomvcdata.Data;
using demomvcdata.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text.Json;

namespace demomvcdata.Services;

public class ZonasInsegurasService : IZonasInsegurasService
{
    private const string CacheKeyPrefix = "zonas-inseguras:index";

    private readonly ApplicationDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<ZonasInsegurasService> _logger;

    public ZonasInsegurasService(
        ApplicationDbContext context,
        IDistributedCache cache,
        ILogger<ZonasInsegurasService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<ZonaInsegura>> GetAllAsync(int? nivel = null)
    {
        var cacheKey = BuildCacheKey(nivel);
        var zonasDesdeCache = await TryGetFromCacheAsync(cacheKey);
        if (zonasDesdeCache != null)
        {
            _logger.LogInformation("Zonas inseguras obtenidas desde la caché.");
            return zonasDesdeCache;
        }

        IQueryable<ZonaInsegura> zonas = _context.ZonasInseguras.AsNoTracking();

        if (nivel.HasValue)
        {
            zonas = zonas.Where(z => z.NivelPeligro == nivel.Value);
        }

        var resultado = await zonas.ToListAsync();
        await TrySetInCacheAsync(cacheKey, resultado);

        return resultado;
    }

    public Task<ZonaInsegura?> GetByIdAsync(int id)
    {
        return _context.ZonasInseguras
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<ZonaInsegura> CreateAsync(ZonaInsegura zonaInsegura)
    {
        _context.Add(zonaInsegura);
        await _context.SaveChangesAsync();
        await InvalidateIndexCacheAsync();

        return zonaInsegura;
    }

    public async Task<bool> UpdateAsync(int id, ZonaInsegura zonaInsegura)
    {
        var existing = await _context.ZonasInseguras.FirstOrDefaultAsync(z => z.Id == id);
        if (existing == null)
        {
            return false;
        }

        zonaInsegura.Id = id;
        _context.Entry(existing).CurrentValues.SetValues(zonaInsegura);
        await _context.SaveChangesAsync();
        await InvalidateIndexCacheAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var zonaInsegura = await _context.ZonasInseguras.FindAsync(id);
        if (zonaInsegura == null)
        {
            return false;
        }

        _context.ZonasInseguras.Remove(zonaInsegura);
        await _context.SaveChangesAsync();
        await InvalidateIndexCacheAsync();

        return true;
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

    private async Task<List<ZonaInsegura>?> TryGetFromCacheAsync(string cacheKey)
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

    private async Task TrySetInCacheAsync(string cacheKey, List<ZonaInsegura> zonas)
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