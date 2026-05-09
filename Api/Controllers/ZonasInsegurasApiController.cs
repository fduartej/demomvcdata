using demomvcdata.Models;
using demomvcdata.Services;
using Microsoft.AspNetCore.Mvc;

namespace demomvcdata.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZonasInsegurasApiController : ControllerBase
{
    private readonly IZonasInsegurasService _zonasInsegurasService;

    public ZonasInsegurasApiController(IZonasInsegurasService zonasInsegurasService)
    {
        _zonasInsegurasService = zonasInsegurasService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ZonaInsegura>>> GetAll([FromQuery] int? nivel)
    {
        var zonas = await _zonasInsegurasService.GetAllAsync(nivel);
        return Ok(zonas);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ZonaInsegura>> GetById(int id)
    {
        var zona = await _zonasInsegurasService.GetByIdAsync(id);
        if (zona == null)
        {
            return NotFound();
        }

        return Ok(zona);
    }

    [HttpPost]
    public async Task<ActionResult<ZonaInsegura>> Create([FromBody] ZonaInsegura zonaInsegura)
    {
        var created = await _zonasInsegurasService.CreateAsync(zonaInsegura);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ZonaInsegura zonaInsegura)
    {
        if (id != zonaInsegura.Id)
        {
            return BadRequest("El id de la ruta y el cuerpo no coinciden.");
        }

        var updated = await _zonasInsegurasService.UpdateAsync(id, zonaInsegura);
        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }
}