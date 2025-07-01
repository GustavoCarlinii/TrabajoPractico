
using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Clave { get; set; } // En sistemas reales, esto debería ser un hash
   
        [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
        public string Tipo { get; set; } = "usuario"; // Valor predeterminado
        
        public Usuario() { }
    
    }

}
