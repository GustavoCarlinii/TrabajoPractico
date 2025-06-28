using CrudMVCApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;

namespace CrudMVCApp.Controllers
{
    public class LoginController : Controller
    {
        
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombreUsuario, string clave)
        {
            // Aca abajo iria el AppDbContext db
            var usuario = _context.Usuario.FirstOrDefault(u => u.nombre == nombreUsuario && u.Clave == clave);
            if (usuario != null)
            {
                HttpContext.Session.SetString("Usuario", usuario.nombre);
                HttpContext.Session.SetString("Tipo", usuario.Tipo);
                
                if (usuario.Tipo == "admin")
                    return RedirectToAction("VistaAdmin", "Login");
                else
                    return RedirectToAction("VistaUsuario", "Login");
            }

            ViewBag.Error = "Usuario o clave incorrecta";
            return View();
        }

        public IActionResult VistaUsuario()
        {
            if (HttpContext.Session.GetString("Rol") != "usuario")
                return RedirectToAction("Login");

            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
            return View();
        }

        public IActionResult VistaAdmin()
        {
            if (HttpContext.Session.GetString("Rol") != "admin")
                return RedirectToAction("Login");

            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
            return View();
        }


        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
