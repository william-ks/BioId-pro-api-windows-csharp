using BiometricAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BiometricAPI.Data
{
    /// <summary>
    /// Contexto do banco de dados para armazenamento de biometrias
    /// </summary>
    public class BiometricDbContext : DbContext
    {
        public BiometricDbContext(DbContextOptions<BiometricDbContext> options)
            : base(options)
        {
        }

        public DbSet<BiometricTemplate> BiometricTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade BiometricTemplate
            modelBuilder.Entity<BiometricTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TemplateBase64).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });
        }
    }
}
