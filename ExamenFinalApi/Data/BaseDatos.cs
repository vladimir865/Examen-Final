using Microsoft.EntityFrameworkCore;
using ExamenFinalApi.Models;

namespace ExamenFinalApi.Data
{
    public class BaseDatos : DbContext
    {
        public BaseDatos(DbContextOptions<BaseDatos> options) : base(options) { }

        public DbSet<Jefe> Jefes { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Jefe>(entity =>
            {
                entity.ToTable("jefes");
                entity.Property(j => j.Nombre).HasMaxLength(100);
                entity.Property(j => j.Cargo).HasMaxLength(100);
                entity.Property(j => j.Correo).HasMaxLength(100);
                entity.Property(j => j.Foto).HasMaxLength(300);

                // Regla: no puede haber 2 jefes con el mismo nombre
                entity.HasIndex(j => j.Nombre).IsUnique();
            });

            modelBuilder.Entity<Proyecto>(entity =>
            {
                entity.ToTable("proyectos");
                entity.Property(p => p.Nombre).HasMaxLength(100);
                entity.Property(p => p.Descripcion).HasMaxLength(500);
                entity.Property(p => p.Estado).HasMaxLength(30);

                // Regla: no puede haber 2 proyectos con el mismo nombre
                entity.HasIndex(p => p.Nombre).IsUnique();

                // Relación 1 a 1: un proyecto tiene un jefe y un jefe solo un proyecto.
                // HasForeignKey<Proyecto> hace que IdJefe sea ÚNICO en la base.
                // Restrict: no se puede borrar un jefe que está a cargo de un proyecto.
                entity.HasOne(p => p.Jefe)
                      .WithOne(j => j.Proyecto)
                      .HasForeignKey<Proyecto>(p => p.IdJefe)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuarios");
                entity.Property(u => u.NombreUsuario).HasMaxLength(50);
                entity.Property(u => u.PasswordHash).HasMaxLength(200);
                entity.Property(u => u.Rol).HasMaxLength(30);
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
            });
        }
    }
}
