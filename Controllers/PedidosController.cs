
using CrudMVCApp.Data;
using CrudMVCApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PedidosController : Controller
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    
    public async Task<IActionResult> Index(string buscarUsuario)
    {
        var pedidos = from p in _context.Pedido
                      select p;

        if (!string.IsNullOrEmpty(buscarUsuario))
        {
            pedidos = pedidos.Where(p => p.Usuario.Contains(buscarUsuario));
        }

        return View(await pedidos.ToListAsync());
    }

    
    public async Task<IActionResult> Detalle(int id)
    {
        var pedido = await _context.Pedido.FindAsync(id);
        if (pedido == null) return NotFound();

        return View(pedido);
    }
    // GET: Mostrar formulario
    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    // POST: Guardar pedido
    [HttpPost]
    public async Task<IActionResult> Crear(Pedido pedido)
    {
        if (ModelState.IsValid)
        {
            pedido.Fecha = DateTime.Now; // Fecha actual
            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(pedido);
    }

}
