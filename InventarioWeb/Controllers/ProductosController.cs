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
    public class ProductosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Productos
        public async Task<IActionResult> Index(int? pageNumber, string? searchString, DateTime? fechaDesde, DateTime? fechaHasta, string? sortOrder)
        {
            if (pageNumber == null || pageNumber < 1)
            {
                pageNumber = 1;
            }

            // Guardar el estado de filtrado y ordenamiento en ViewData
            ViewData["CurrentFilter"] = searchString;
            ViewData["FechaDesde"] = fechaDesde?.ToString("yyyy-MM-dd");
            ViewData["FechaHasta"] = fechaHasta?.ToString("yyyy-MM-dd");
            ViewData["CurrentSort"] = sortOrder;

            var query = _context.Productos.AsQueryable();

            // Filtros de busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Nombre.Contains(searchString) ||
                    p.SKU.Contains(searchString) ||
                    p.Responsable.Contains(searchString));
            }

            // Filtros por fecha
            if (fechaDesde.HasValue)
            {
                query = query.Where(p => p.Fecha >= fechaDesde.Value);
            }

            if (fechaHasta.HasValue)
            {
                query = query.Where(p => p.Fecha <= fechaHasta.Value);
            }

            // orden
            switch (sortOrder)
            {
                case "stock_asc":
                    query = query.OrderBy(p => p.Cantidad);
                    break;
                case "stock_desc":
                    query = query.OrderByDescending(p => p.Cantidad);
                    break;
                case "nombre_asc":
                    query = query.OrderBy(p => p.Nombre);
                    break;
                case "nombre_desc":
                    query = query.OrderByDescending(p => p.Nombre);
                    break;
                default:
                    query = query.OrderByDescending(p => p.Fecha); // Por defecto, más reciente primero
                    break;
            }

            int pageSize = 5;
            var productos = await PaginatedList<Producto>.CreateAsync(
                query,
                pageNumber ?? 1,
                pageSize);

            return View(productos);
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.ProductoID == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoID,Nombre,SKU,Valor,Cantidad,Responsable,StockMinimo,Fecha")] Producto producto)
        {
            if (!ModelState.IsValid)
            {
                // Comprobaciones
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(producto);
            }

            _context.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoID,Nombre,SKU,Valor,Cantidad,Responsable,StockMinimo,Fecha")] Producto producto)
        {
            if (id != producto.ProductoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoID))
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
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.ProductoID == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoID == id);
        }
    }
}
