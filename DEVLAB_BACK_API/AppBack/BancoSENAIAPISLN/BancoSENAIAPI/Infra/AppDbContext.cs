using BancoSENAIAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Infra
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Agencia> Agencia { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Como você não usou o nome padrão "Id", precisamos avisar ao EF qual é a Chave Primária
            modelBuilder.Entity<Agencia>()
                .HasKey(a => a.NumeroAgencia);

            base.OnModelCreating(modelBuilder);
        }
    }
}
