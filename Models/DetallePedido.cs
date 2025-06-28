using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudMVCApp.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }
        
        [Required]
        public int PedidoId { get; set; }
        
        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; }
        
        [Required]
        public int ProductoId { get; set; }
        
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; }
        
        [Required]
        public int Cantidad { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnitario { get; set; }
        
        [NotMapped]
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}