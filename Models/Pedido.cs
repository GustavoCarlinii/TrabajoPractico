using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudMVCApp.Models
{
    public class Pedido
    {
        public int id { get; set; }
        
        public string Usuario { get; set; }
        
        public DateTime Fecha { get; set; }
        
        [Required]
        public int ClienteId { get; set; }
        
        [ForeignKey("ClienteId")]
        public Persona Cliente { get; set; }
        
        public bool Confirmado { get; set; } = false;
        
        public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
        
        [NotMapped]
        public decimal Total => Detalles?.Sum(d => d.Subtotal) ?? 0;
        
        [NotMapped]
        public int TotalProductos => Detalles?.Sum(d => d.Cantidad) ?? 0;
    }
}
