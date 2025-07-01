using CrudMVCApp.Data;
using CrudMVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Usuario model)
        {
            // Asegurar que el tipo tenga un valor (por defecto "usuario")
            if (string.IsNullOrEmpty(model.Tipo))
            {
                model.Tipo = "usuario";
            }
            
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuario
                    .FirstOrDefault(u => u.nombre == model.nombre && u.Clave == model.Clave);
                
                if (usuario != null)
                {
                    HttpContext.Session.SetString("Usuario", usuario.nombre);
                    HttpContext.Session.SetString("Rol", usuario.Tipo); // "admin" o "usuario"
                    
                    // Redirigir a la página de inicio
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos. Verifique sus credenciales.");
                }
            }
            else
            {
                // Agregar errores específicos al ModelState para mostrarlos en la vista
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
            }

            return View(model);
        }

        public IActionResult VistaAdmin()
        {
            if (HttpContext.Session.GetString("Rol") != "admin")
                return RedirectToAction("Login");

            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
            return View();
        }

        public IActionResult VistaUsuario()
        {
            if (HttpContext.Session.GetString("Rol") != "usuario")
                return RedirectToAction("Login");

            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
            return View();
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
