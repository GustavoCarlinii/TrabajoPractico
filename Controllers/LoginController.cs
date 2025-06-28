using CrudMVCApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;

namespace CrudMVCApp.Controllers
{
    public class LoginController : Controller
    {
        private AppDbContext db = new AppDbContext();


       [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        

        [HttpPost]
        public IActionResult Login(string nombreUsuario, string clave)
        {
            var usuario = db.Usuario.FirstOrDefault(u => u.user == nombreUsuario && u.Clave == clave);
            if (usuario != null)
            {
                HttpContext.Session.SetString("Usuario", usuario.user);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o clave incorrecta";
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
