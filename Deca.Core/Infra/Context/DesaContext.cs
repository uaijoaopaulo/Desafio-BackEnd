using Desa.Core.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Desa.Core.Infra.Context
{
    public class DesaContext : DbContext
    {
        public DbSet<MotoModel> Motos { get; set; }
        public DbSet<EntregadorModel> Entregadores { get; set; }
        public DbSet<PedidoModel> Pedidos { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<LocacaoModel> Locacoes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = (new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)).Build();

            optionsBuilder.UseNpgsql(config.GetConnectionString("SQLServer"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MotoModel>(_ =>
            {
                _.HasKey(p => p.Id);
                _.HasIndex(p => p.LicensePlate).IsUnique();
            });
            
            modelBuilder.Entity<EntregadorModel>(_ =>
            {
                _.HasKey(p => p.Id);
                _.HasIndex(p => p.CNPJ).IsUnique();
                _.HasIndex(p => p.CNH).IsUnique();
            });

            modelBuilder.Entity<PedidoModel>(_ =>
            {
                _.HasKey(p => p.Id);
            });

            modelBuilder.Entity<UserModel>(_ =>
            {
                _.HasKey(p => p.Id);
            }); 
            
            modelBuilder.Entity<LocacaoModel>(_ =>
            {
                _.HasKey(p => p.Id);
            });
        }
    }
}
