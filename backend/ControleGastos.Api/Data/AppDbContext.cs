using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleGastos.Api.Data;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Pessoa>().Property(x => x.Nome).HasMaxLength(150).IsRequired();
        b.Entity<Transacao>().Property(x => x.Descricao).HasMaxLength(250).IsRequired();
        b.Entity<Transacao>().Property(x => x.Valor).HasPrecision(18, 2);
        // A cascata garante a remoção das transações também no nível do banco.
        b.Entity<Pessoa>().HasMany(x => x.Transacoes).WithOne(x => x.Pessoa).HasForeignKey(x => x.PessoaId).OnDelete(DeleteBehavior.Cascade);
    }
}
