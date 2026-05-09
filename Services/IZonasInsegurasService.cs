using demomvcdata.Models;

namespace demomvcdata.Services;

public interface IZonasInsegurasService
{
    Task<List<ZonaInsegura>> GetAllAsync(int? nivel = null);
    Task<ZonaInsegura?> GetByIdAsync(int id);
    Task<ZonaInsegura> CreateAsync(ZonaInsegura zonaInsegura);
    Task<bool> UpdateAsync(int id, ZonaInsegura zonaInsegura);
    Task<bool> DeleteAsync(int id);
}