using System.ComponentModel.DataAnnotations;

namespace InventarioWeb.Models
{
    public class Producto
    {
        public Producto()
        {
            Entradas = new List<Entrada>();
            Salidas = new List<Salida>();
        }

        public int ProductoID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El SKU es obligatorio.")]
        [StringLength(50, ErrorMessage = "El SKU no puede tener más de 50 caracteres.")]
        public string SKU { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio.")]
        [Range(0, 9999999999.99, ErrorMessage = "El valor debe ser un número positivo.")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser un número positivo.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El responsable es obligatorio.")]
        [StringLength(50, ErrorMessage = "El responsable no puede tener más de 50 caracteres.")]
        public string Responsable { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Display(Name = "Stock mínimo")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser un número positivo.")]
        public int StockMinimo { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria.")]
        public DateTime Fecha { get; set; }
        public ICollection<Entrada> Entradas { get; set; }
        public ICollection<Salida> Salidas { get; set; }
    }
}
