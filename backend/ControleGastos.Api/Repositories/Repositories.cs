using ControleGastos.Api.Data;
using ControleGastos.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace ControleGastos.Api.Repositories;

public interface IPessoaRepository
{
    Task<List<Pessoa>> ListarAsync(CancellationToken ct); Task<Pessoa?> ObterAsync(int id, CancellationToken ct); Task<Pessoa> CriarAsync(Pessoa pessoa, CancellationToken ct); Task ExcluirAsync(Pessoa pessoa, CancellationToken ct);
}
public class PessoaRepository(AppDbContext db) : IPessoaRepository
{
    public Task<List<Pessoa>> ListarAsync(CancellationToken ct) => db.Pessoas.AsNoTracking().OrderBy(x => x.Nome).ToListAsync(ct);
    public Task<Pessoa?> ObterAsync(int id, CancellationToken ct) => db.Pessoas.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<Pessoa> CriarAsync(Pessoa p, CancellationToken ct) { db.Add(p); await db.SaveChangesAsync(ct); return p; }
    public async Task ExcluirAsync(Pessoa p, CancellationToken ct) { db.Remove(p); await db.SaveChangesAsync(ct); }
}
public interface ITransacaoRepository
{
    Task<List<Transacao>> ListarAsync(int? pessoaId, CancellationToken ct); Task<Transacao> CriarAsync(Transacao transacao, CancellationToken ct);
}
public class TransacaoRepository(AppDbContext db) : ITransacaoRepository
{
    public Task<List<Transacao>> ListarAsync(int? pessoaId, CancellationToken ct) => db.Transacoes.AsNoTracking().Include(x => x.Pessoa).Where(x => !pessoaId.HasValue || x.PessoaId == pessoaId).OrderByDescending(x => x.Id).ToListAsync(ct);
    public async Task<Transacao> CriarAsync(Transacao t, CancellationToken ct) { db.Add(t); await db.SaveChangesAsync(ct); return t; }
}
