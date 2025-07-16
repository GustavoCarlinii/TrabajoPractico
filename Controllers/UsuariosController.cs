using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudMVCApp.Data;
using CrudMVCApp.Models;
using Microsoft.AspNetCore.Http;

namespace CrudMVCApp.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetString("Rol") == "admin";
        }

        private IActionResult RedireccionSiNoAdmin()
        {
            if (!EsAdmin())
                return RedirectToAction("Login", "Login");

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            return View(await _context.Usuario.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            if (id == null) return NotFound();

            var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        public IActionResult Create()
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,nombre,Clave,Tipo")] Usuario usuario)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            var usuarioExistente = await _context.Usuario
                .FirstOrDefaultAsync(u => u.nombre == usuario.nombre);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError("nombre", "Este nombre de usuario ya está en uso");
                TempData["Error"] = "El nombre de usuario ya está registrado";
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuario creado correctamente";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            if (id == null) return NotFound();

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,nombre,Clave,Tipo")] Usuario usuario)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            if (id == null) return NotFound();

            var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var redir = RedireccionSiNoAdmin();
            if (redir != null) return redir;

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
