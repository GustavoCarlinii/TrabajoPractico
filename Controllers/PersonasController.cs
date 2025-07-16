using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrudMVCApp.Data;
using CrudMVCApp.Models;

namespace CrudMVCApp.Controllers
{
    public class PersonasController : Controller
    {
        private readonly AppDbContext _context;

        public PersonasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Personas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Persona.ToListAsync());
        }

        // GET: Personas/Details/5
        public async Task<IActionResult> Details(int? id) //2
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                //.FindAsync(id) nos permite solo busquedas por PK
                .FirstOrDefaultAsync(m => m.Id == id);
              //  (p => p.Dni == DNI && p.Genero == "F")
                
            // Select Top 1 * From Persona where id= 2
            // lambda
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // GET: Personas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Dni,Cuit,Direccion,Telefono,Email")] Persona persona)
        {
            // Verificar si el DNI ya existe
            var personaExistenteDni = await _context.Persona.FirstOrDefaultAsync(p => p.Dni == persona.Dni);
            if (personaExistenteDni != null)
            {
                ModelState.AddModelError("Dni", "Este DNI ya está registrado");
                TempData["Error"] = "El DNI ya está registrado en el sistema";
                return View(persona);
            }

            // Verificar si el CUIT ya existe
            if (!string.IsNullOrEmpty(persona.Cuit))
            {
                var personaExistenteCuit = await _context.Persona.FirstOrDefaultAsync(p => p.Cuit == persona.Cuit);
                if (personaExistenteCuit != null)
                {
                    ModelState.AddModelError("Cuit", "Este CUIT ya está registrado");
                    TempData["Error"] = "El CUIT ya está registrado en el sistema";
                    return View(persona);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(persona);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente registrado correctamente";
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // GET: Personas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona.FindAsync(id);
            if (persona == null)
            {
                return NotFound();
            }
            return View(persona);
        }

        // POST: Personas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Dni,Cuit,Futbol,Basquet,Otros,Genero")] Persona persona)
        {
            if (id != persona.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(persona);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonaExists(persona.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(persona);
        }

        // GET: Personas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _context.Persona
                .FirstOrDefaultAsync(m => m.Id == id);
            if (persona == null)
            {
                return NotFound();
            }

            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Persona.FindAsync(id);
            if (persona != null)
            {
                _context.Persona.Remove(persona);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Persona.Any(e => e.Id == id);
        }
    }
}
