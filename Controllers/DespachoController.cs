using CrudMVCApp.Data;
using CrudMVCApp.Models;
using CrudMVCApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CrudMVCApp.Controllers
{
    public class DespachoController : Controller
    {
        private readonly AppDbContext _context;

        public DespachoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Mostrar listado de despachos
        public async Task<IActionResult> Index(string buscarCliente)
        {
            var pedidos = _context.Pedido
                .Include(p => p.Cliente)
                .AsQueryable();

            if (!string.IsNullOrEmpty(buscarCliente))
            {
                pedidos = pedidos.Where(p => p.Cliente.Nombre.Contains(buscarCliente) || 
                                           p.Cliente.Apellido.Contains(buscarCliente));
            }

            return View(await pedidos.ToListAsync());
        }

        // GET: Mostrar formulario de nuevo despacho
        public async Task<IActionResult> Crear()
        {
            var viewModel = new PedidoViewModel
            {
                Usuario = HttpContext.Session.GetString("Usuario") ?? "Sistema",
                Clientes = await _context.Persona.ToListAsync(),
                Productos = await _context.Producto.ToListAsync()
            };

            // Guardar la lista de detalles vacía en TempData para iniciar
            TempData["DetallesPedido"] = JsonSerializer.Serialize(new List<DetallePedidoViewModel>());

            return View(viewModel);
        }

        // POST: Agregar producto al detalle
        [HttpPost]
        public async Task<IActionResult> AgregarProducto(int productoId, int cantidad)
        {
            if (cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a cero");
            }

            var producto = await _context.Producto.FindAsync(productoId);
            if (producto == null)
            {
                return NotFound("Producto no encontrado");
            }

            // Recuperar la lista actual de detalles
            var detallesJson = TempData["DetallesPedido"] as string;
            var detalles = string.IsNullOrEmpty(detallesJson) 
                ? new List<DetallePedidoViewModel>() 
                : JsonSerializer.Deserialize<List<DetallePedidoViewModel>>(detallesJson);

            // Verificar si el producto ya existe en el detalle
            var detalleExistente = detalles.FirstOrDefault(d => d.ProductoId == productoId);
            if (detalleExistente != null)
            {
                // Actualizar cantidad y subtotal
                detalleExistente.Cantidad += cantidad;
            }
            else
            {
                // Agregar nuevo detalle
                detalles.Add(new DetallePedidoViewModel
                {
                    ProductoId = producto.Id,
                    Descripcion = producto.Nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = (decimal)producto.PrecioVta
                });
            }

            // Guardar la lista actualizada en TempData
            TempData["DetallesPedido"] = JsonSerializer.Serialize(detalles);

            // Redirigir a la vista de creación
            return RedirectToAction("Crear");
        }

        // POST: Eliminar producto del detalle
        [HttpPost]
        public IActionResult EliminarProducto(int productoId)
        {
            // Recuperar la lista actual de detalles
            var detallesJson = TempData["DetallesPedido"] as string;
            var detalles = string.IsNullOrEmpty(detallesJson) 
                ? new List<DetallePedidoViewModel>() 
                : JsonSerializer.Deserialize<List<DetallePedidoViewModel>>(detallesJson);

            // Eliminar el producto del detalle
            detalles.RemoveAll(d => d.ProductoId == productoId);

            // Guardar la lista actualizada en TempData
            TempData["DetallesPedido"] = JsonSerializer.Serialize(detalles);

            // Redirigir a la vista de creación
            return RedirectToAction("Crear");
        }

        // POST: Confirmar pedido
        [HttpPost]
        public async Task<IActionResult> ConfirmarPedido(PedidoViewModel viewModel)
        {
            if (viewModel.ClienteId <= 0)
            {
                ModelState.AddModelError("ClienteId", "Debe seleccionar un cliente");
                viewModel.Clientes = await _context.Persona.ToListAsync();
                viewModel.Productos = await _context.Producto.ToListAsync();
                return View("Crear", viewModel);
            }

            // Recuperar la lista de detalles
            var detallesJson = TempData["DetallesPedido"] as string;
            var detallesViewModel = string.IsNullOrEmpty(detallesJson) 
                ? new List<DetallePedidoViewModel>() 
                : JsonSerializer.Deserialize<List<DetallePedidoViewModel>>(detallesJson);

            if (detallesViewModel.Count == 0)
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto al pedido");
                viewModel.Clientes = await _context.Persona.ToListAsync();
                viewModel.Productos = await _context.Producto.ToListAsync();
                return View("Crear", viewModel);
            }

            // Crear el pedido
            var pedido = new Pedido
            {
                ClienteId = viewModel.ClienteId,
                Usuario = HttpContext.Session.GetString("Usuario") ?? "Sistema",
                Fecha = DateTime.Now,
                Confirmado = true,
                Detalles = new List<DetallePedido>()
            };

            // Agregar los detalles al pedido
            foreach (var detalle in detallesViewModel)
            {
                pedido.Detalles.Add(new DetallePedido
                {
                    ProductoId = detalle.ProductoId,
                    Cantidad = detalle.Cantidad,
                    PrecioUnitario = detalle.PrecioUnitario
                });

                // Actualizar stock del producto
                var producto = await _context.Producto.FindAsync(detalle.ProductoId);
                if (producto != null)
                {
                    producto.Stock -= detalle.Cantidad;
                    _context.Update(producto);
                }
            }

            // Guardar en la base de datos
            _context.Add(pedido);
            await _context.SaveChangesAsync();

            // Limpiar TempData
            TempData.Remove("DetallesPedido");

            return RedirectToAction("Detalle", new { id = pedido.id });
        }

        // GET: Ver detalle de un despacho
        public async Task<IActionResult> Detalle(int id)
        {
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