using Microsoft.EntityFrameworkCore;
using VentifyAPI.Models;
using VentifyAPI.Services;

namespace VentifyAPI.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantContext _tenant;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant) : base(options)
        {
            _tenant = tenant;
        }

        public DbSet<Negocio> Negocios => Set<Negocio>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Negocio>(entity =>
            {
                entity.ToTable("negocios");
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Rol).HasDefaultValue("dueno");
                entity.HasIndex(e => e.Correo).IsUnique();
            });

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired();
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Negocio)
                    .WithMany()
                    .HasForeignKey(e => e.NegocioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("ventas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FormaPago).HasDefaultValue("efectivo");
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId);
                entity.HasOne(e => e.Negocio)
                    .WithMany()
                    .HasForeignKey(e => e.NegocioId);
            });

            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.ToTable("detalles_venta");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Venta)
                    .WithMany(v => v.Detalles)
                    .HasForeignKey(e => e.VentaId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany(p => p.Detalles)
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Usuario>().HasQueryFilter(u => _tenant.NegocioId == null || u.NegocioId == _tenant.NegocioId);
            modelBuilder.Entity<Producto>().HasQueryFilter(p => _tenant.NegocioId == null || p.NegocioId == _tenant.NegocioId);
            modelBuilder.Entity<Venta>().HasQueryFilter(v => _tenant.NegocioId == null || v.NegocioId == _tenant.NegocioId);
            modelBuilder.Entity<DetalleVenta>().HasQueryFilter(d => _tenant.NegocioId == null || d.Venta != null && d.Venta.NegocioId == _tenant.NegocioId);
            modelBuilder.Entity<RefreshToken>().HasQueryFilter(r => _tenant.NegocioId == null || r.Usuario != null && r.Usuario.NegocioId == _tenant.NegocioId);
        }
    }
}
