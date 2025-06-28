namespace CrudMVCApp.Models
{
    public class Pedido
    {
        public int id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalles { get; set; }
    }
}
