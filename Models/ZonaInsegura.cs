using System.ComponentModel.DataAnnotations;

namespace demomvcdata.Models;

public class ZonaInsegura
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
    [Display(Name = "Nombre de la Zona")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La dirección es obligatoria")]
    [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
    [Display(Name = "Dirección")]
    public string Direccion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nivel de peligro es obligatorio")]
    [Display(Name = "Nivel de Peligro")]
    [Range(1, 5, ErrorMessage = "El nivel de peligro debe estar entre 1 y 5")]
    public int NivelPeligro { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Display(Name = "Fecha de Registro")]
    [DataType(DataType.Date)]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Display(Name = "Zona Activa")]
    public bool Activa { get; set; } = true;
}
