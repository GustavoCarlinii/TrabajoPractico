using System.ComponentModel.DataAnnotations;

namespace CrudMVCApp.Models
{
    public class Persona
    {
        public int Id { get; set; } // Clave primaria necesaria para EF
        public string Nombre { get; set; }
       
        [Display(Name = "Apellido del Cliente")]
        public string Apellido { get; set; }
        
        public int Dni { get; set; }
        public string Cuit { get; set; }
        public bool Futbol { get; set; }
        public bool Basquet { get; set; }
        public bool Otros { get; set; }
        public char Genero { get; set; }

        //Propiedad navegacion 
        public ICollection<Direccion> Direcciones { get; set; }
        public Persona (){}
        
    }
}

