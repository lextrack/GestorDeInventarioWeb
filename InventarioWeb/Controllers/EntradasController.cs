using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioWeb.Data;
using InventarioWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace InventarioWeb.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class EntradasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntradasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Entradas
        public async Task<IActionResult> Index(int? pageNumber, string? searchString, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (pageNumber == null || pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = _context.Entradas
                .Include(e => e.Producto)
                .AsQueryable();

            // Búsqueda por producto o observación
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(e =>
                    e.Producto.Nombre.Contains(searchString) ||
                    e.Observacion.Contains(searchString));
            }

            // Filtro por rango de fechas
            if (fechaDesde.HasValue)
            {
                query = query.Where(e => e.Fecha >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(e => e.Fecha <= fechaHasta.Value);
            }

            int pageSize = 10; // Entradas por página
            var entradas = await PaginatedList<Entrada>.CreateAsync(
                query.OrderByDescending(e => e.Fecha),
                pageNumber ?? 1,
                pageSize);

            return View(entradas);
        }

        // GET: Entradas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas
                .Include(e => e.Producto)
                .FirstOrDefaultAsync(m => m.EntradaID == id);
            if (entrada == null)
            {
                return NotFound();
            }

            return View(entrada);
        }

        // GET: Entradas/Create
        public IActionResult Create()
        {
            ViewData["ProductoID"] = new SelectList(_context.Productos, "ProductoID", "Nombre");
            return View();
        }

        // POST: Entradas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntradaID,Fecha,Cantidad,ProductoID,Observacion")] Entrada entrada)
        {
            if (ModelState.IsValid)
            {
                // Agregar la entrada
                _context.Add(entrada);
                await _context.SaveChangesAsync();

                // Actualizar la cantidad del producto
                var producto = await _context.Productos.FindAsync(entrada.ProductoID);
                if (producto != null)
                {
                    producto.Cantidad += entrada.Cantidad;
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(entrada);
        }

        // GET: Entradas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas.FindAsync(id);
            if (entrada == null)
            {
                return NotFound();
            }
            ViewData["ProductoID"] = new SelectList(_context.Productos, "ProductoID", "Nombre", entrada.ProductoID);
            return View(entrada);
        }

        // POST: Entradas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EntradaID,Fecha,Cantidad,ProductoID,Observacion")] Entrada entrada)
        {
            if (id != entrada.EntradaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la entrada original antes de la actualización
                    var entradaOriginal = await _context.Entradas.AsNoTracking().FirstOrDefaultAsync(e => e.EntradaID == id);

                    // Actualizar la entrada
                    _context.Update(entrada);
                    await _context.SaveChangesAsync();

                    // Actualizar la cantidad del producto
                    var producto = await _context.Productos.FindAsync(entrada.ProductoID);
                    if (producto != null && entradaOriginal != null)
                    {
                        producto.Cantidad += (entrada.Cantidad - entradaOriginal.Cantidad);
                        _context.Update(producto);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntradaExists(entrada.EntradaID))
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
            return View(entrada);
        }

        // GET: Entradas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas
                .Include(e => e.Producto)
                .FirstOrDefaultAsync(m => m.EntradaID == id);
            if (entrada == null)
            {
                return NotFound();
            }

            return View(entrada);
        }

        // POST: Entradas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entrada = await _context.Entradas.FindAsync(id);
            if (entrada != null)
            {
                _context.Entradas.Remove(entrada);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntradaExists(int id)
        {
            return _context.Entradas.Any(e => e.EntradaID == id);
        }
    }
}
