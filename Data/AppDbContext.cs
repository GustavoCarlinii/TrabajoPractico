using Microsoft.EntityFrameworkCore;
using CrudMVCApp.Models;

namespace CrudMVCApp.Data
{
    public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Persona> Persona { get; set; }
    public DbSet<Producto> Producto { get; set; }
    public DbSet<Direccion> Direccion { get; set; }
    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Pedido> Pedido { get; set; }
    public DbSet<DetallePedido> DetallePedido { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Persona>()
            .HasMany(p => p.Direcciones)
            .WithOne(d => d.Persona)
            .HasForeignKey(d => d.PersonaId);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Detalles)
            .WithOne(d => d.Pedido)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Cliente)
            .WithMany()
            .HasForeignKey(p => p.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

}
