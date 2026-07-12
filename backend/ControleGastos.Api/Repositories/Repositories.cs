using ControleGastos.Api.Data;
using ControleGastos.Api.Exceptions;
using ControleGastos.Api.Enums;
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
    public async Task<Pessoa> CriarAsync(Pessoa p, CancellationToken ct)
    {
        db.Add(p);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex)
        {
            throw new AppException($"Não foi possível salvar a pessoa: {ex.InnerException?.Message ?? ex.Message}");
        }
        return p;
    }
    public async Task ExcluirAsync(Pessoa p, CancellationToken ct) { db.Remove(p); await db.SaveChangesAsync(ct); }
}
public interface ITransacaoRepository
{
    Task<List<Transacao>> ListarAsync(int? pessoaId, TipoTransacao? tipo, CategoriaTransacao? categoria, DateTime? inicio, DateTime? fim, string? busca, CancellationToken ct); Task<Transacao?> ObterAsync(int id, CancellationToken ct); Task<Transacao> CriarAsync(Transacao transacao, CancellationToken ct); Task ExcluirAsync(Transacao transacao, CancellationToken ct);
}
public class TransacaoRepository(AppDbContext db) : ITransacaoRepository
{
    public Task<List<Transacao>> ListarAsync(int? pessoaId, TipoTransacao? tipo, CategoriaTransacao? categoria, DateTime? inicio, DateTime? fim, string? busca, CancellationToken ct)
    {
        var termo = busca?.Trim();
        return db.Transacoes.AsNoTracking().Include(x => x.Pessoa)
            .Where(x => !pessoaId.HasValue || x.PessoaId == pessoaId)
            .Where(x => !tipo.HasValue || x.Tipo == tipo)
            .Where(x => !categoria.HasValue || x.Categoria == categoria)
            .Where(x => !inicio.HasValue || x.Data >= inicio.Value.Date)
            .Where(x => !fim.HasValue || x.Data < fim.Value.Date.AddDays(1))
            .Where(x => string.IsNullOrEmpty(termo) || x.Descricao.Contains(termo))
            .OrderByDescending(x => x.Data).ThenByDescending(x => x.Id).ToListAsync(ct);
    }
    public Task<Transacao?> ObterAsync(int id, CancellationToken ct) => db.Transacoes.FirstOrDefaultAsync(x => x.Id == id, ct);
    public async Task<Transacao> CriarAsync(Transacao t, CancellationToken ct)
    {
        db.Add(t);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException ex)
        {
            throw new AppException($"Não foi possível salvar a transação: {ex.InnerException?.Message ?? ex.Message}");
        }
        return t;
    }
    public async Task ExcluirAsync(Transacao transacao, CancellationToken ct) { db.Transacoes.Remove(transacao); await db.SaveChangesAsync(ct); }
}
