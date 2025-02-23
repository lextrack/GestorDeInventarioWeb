using InventarioWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventarioWeb.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
    {
    }

    // DbSets para cada entidad
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Entrada> Entradas { get; set; }
    public DbSet<Salida> Salidas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relaciones
        modelBuilder.Entity<Entrada>()
            .HasOne(e => e.Producto)
            .WithMany(p => p.Entradas)
            .HasForeignKey(e => e.ProductoID);

        modelBuilder.Entity<Salida>()
            .HasOne(s => s.Producto)
            .WithMany(p => p.Salidas)
            .HasForeignKey(s => s.ProductoID);
    }
}

