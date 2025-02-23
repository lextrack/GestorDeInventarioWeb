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
    public class SalidasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalidasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Salidas
        public async Task<IActionResult> Index(int? pageNumber, string? searchString, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (pageNumber == null || pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = _context.Salidas
                .Include(s => s.Producto)
                .AsQueryable();

            // Búsqueda por producto o observación
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s =>
                    s.Producto.Nombre.Contains(searchString) ||
                    s.Observacion.Contains(searchString));
            }

            // Filtro por rango de fechas
            if (fechaDesde.HasValue)
            {
                query = query.Where(s => s.Fecha >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(s => s.Fecha <= fechaHasta.Value);
            }

            int pageSize = 10; // Salidas por página
            var salidas = await PaginatedList<Salida>.CreateAsync(
                query.OrderByDescending(s => s.Fecha),
                pageNumber ?? 1,
                pageSize);

            return View(salidas);
        }

        // GET: Salidas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salida = await _context.Salidas
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(m => m.SalidaID == id);
            if (salida == null)
            {
                return NotFound();
            }

            return View(salida);
        }

        // GET: Salidas/Create
        public IActionResult Create()
        {
            ViewData["ProductoID"] = new SelectList(_context.Productos, "ProductoID", "Nombre");
            return View();
        }

        // POST: Salidas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalidaID,Fecha,Cantidad,ProductoID,Observacion")] Salida salida)
        {
            if (ModelState.IsValid)
            {
                // Agregar la salida
                _context.Add(salida);
                await _context.SaveChangesAsync();

                // Actualizar la cantidad del producto
                var producto = await _context.Productos.FindAsync(salida.ProductoID);
                if (producto != null)
                {
                    producto.Cantidad -= salida.Cantidad;
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            return View(salida);
        }

        // GET: Salidas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salida = await _context.Salidas.FindAsync(id);
            if (salida == null)
            {
                return NotFound();
            }
            ViewData["ProductoID"] = new SelectList(_context.Productos, "ProductoID", "Nombre", salida.ProductoID);
            return View(salida);
        }

        // POST: Salidas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalidaID,Fecha,Cantidad,ProductoID,Observacion")] Salida salida)
        {
            if (id != salida.SalidaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener la salida original antes de la actualización
                    var salidaOriginal = await _context.Salidas.AsNoTracking().FirstOrDefaultAsync(s => s.SalidaID == id);

                    // Actualizar la salida
                    _context.Update(salida);
                    await _context.SaveChangesAsync();

                    // Actualizar la cantidad del producto
                    var producto = await _context.Productos.FindAsync(salida.ProductoID);
                    if (producto != null && salidaOriginal != null)
                    {
                        producto.Cantidad -= (salida.Cantidad - salidaOriginal.Cantidad);
                        _context.Update(producto);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalidaExists(salida.SalidaID))
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
            return View(salida);
        }

        // GET: Salidas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salida = await _context.Salidas
                .Include(s => s.Producto)
                .FirstOrDefaultAsync(m => m.SalidaID == id);
            if (salida == null)
            {
                return NotFound();
            }

            return View(salida);
        }

        // POST: Salidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salida = await _context.Salidas.FindAsync(id);
            if (salida != null)
            {
                _context.Salidas.Remove(salida);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalidaExists(int id)
        {
            return _context.Salidas.Any(e => e.SalidaID == id);
        }
    }
}
