using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace CrudMVCApp.Models
{
    public class Producto
    {
        public int Id { get; set; } 
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double PrecioCompra  { get; set; }
        public double PrecioVta {  get; set; }
        public int Stock { get; set; }

        public Producto() { }
    }
}
