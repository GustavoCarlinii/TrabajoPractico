
using CrudMVCApp.Data;
using CrudMVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudMVCApp.Controllers
{
    public class PedidosController : Controller
    {
        private readonly AppDbContext _context;

        public PedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Pedidos - Listado de pedidos con búsqueda por usuario
        public async Task<IActionResult> Index(string buscarUsuario)
        {
            // Verificar si hay un usuario en sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login", "Login");
            }

            // Fix: Include Detalles to calculate Total correctly
            var pedidosQuery = _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                .AsQueryable();

            // Aplicar filtro de búsqueda si se proporciona
            if (!string.IsNullOrEmpty(buscarUsuario))
            {
                pedidosQuery = pedidosQuery.Where(p => p.Usuario.Contains(buscarUsuario));
            }

            var pedidos = await pedidosQuery.ToListAsync();
            return View(pedidos);
        }

        // GET: Pedidos/Detalle/5 - Ver detalle completo del pedido
        public async Task<IActionResult> Detalle(int id)
        {
            // Verificar si hay un usuario en sesión
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Usuario")))
            {
                return RedirectToAction("Login", "Login");
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(p => p.id == id);

            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }
    }
}
