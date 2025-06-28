using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models.ViewModels
{
    public class PedidoViewModel
    {
        public int id { get; set; }
        
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public int ClienteId { get; set; }
        
        public string Usuario { get; set; }
        
        public bool Confirmado { get; set; } = false;
        
        // Para agregar productos al pedido
        public int ProductoId { get; set; }
        public int Cantidad { get; set; } = 1;
        
        // Listas para la vista
        public List<Persona> Clientes { get; set; } = new List<Persona>();
        public List<Producto> Productos { get; set; } = new List<Producto>();
        public List<DetallePedidoViewModel> Detalles { get; set; } = new List<DetallePedidoViewModel>();
        
        // Totales calculados
        public decimal Total => Detalles?.Sum(d => d.Subtotal) ?? 0;
        public int TotalProductos => Detalles?.Sum(d => d.Cantidad) ?? 0;
    }
    
    public class DetallePedidoViewModel
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }
}