
using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string nombre { get; set; }

        [Required]
        public string Clave { get; set; } 
        public string Tipo { get; set; }
        public Usuario() { }

    }

}
