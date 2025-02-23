using System.ComponentModel.DataAnnotations;

namespace InventarioWeb.Models
{
    public class Entrada
    {
        public int EntradaID { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "La observación es obligatoria")]
        [StringLength(200, ErrorMessage = "La observación no puede tener más de 200 caracteres")]
        public string Observacion { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public int ProductoID { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}
