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
            // Asegurarse de que el modelo tenga un valor para Tipo
            if (string.IsNullOrEmpty(model.Tipo))
            {
                model.Tipo = "usuario";
            }
            
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuario.FirstOrDefault(u => u.nombre == model.nombre && u.Clave == model.Clave);
                if (usuario != null)
                {
                    HttpContext.Session.SetString("Usuario", usuario.nombre);
                    HttpContext.Session.SetString("Tipo", usuario.Tipo); // "admin" o "usuario"

                    if (usuario.Tipo == "admin")
                        return RedirectToAction("VistaAdmin");
                    else
                        return RedirectToAction("VistaUsuario");
                }
                else
                {
                    ModelState.AddModelError("", "Usuario o contraseña incorrectos. Verifique sus credenciales.");
                }
            }
            else
            {
                // Identificar y mostrar errores específicos
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Agregar cada error específico
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
            }

            return View(model);
        }

        public IActionResult VistaAdmin()
        {
            if (HttpContext.Session.GetString("Tipo") != "admin")
                return RedirectToAction("Login");

            ViewBag.Usuario = HttpContext.Session.GetString("Usuario");
            return View();
        }

        public IActionResult VistaUsuario()
        {
            if (HttpContext.Session.GetString("Tipo") != "usuario")
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
