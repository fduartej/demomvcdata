using System.ComponentModel.DataAnnotations;

namespace demomvcdata.Models;

public class MercadoPagoCheckoutViewModel
{
    [Required(ErrorMessage = "El monto es obligatorio")]
    [Range(1, 100000, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; } = 50;

    [Required(ErrorMessage = "El DNI es obligatorio")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 digitos")]
    public string Dni { get; set; } = string.Empty;

    [StringLength(120, ErrorMessage = "La descripcion no debe superar 120 caracteres")]
    public string Descripcion { get; set; } = "Pago de servicio";
}
