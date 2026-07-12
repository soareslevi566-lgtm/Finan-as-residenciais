using ControleGastos.Api.Data;
using ControleGastos.Api.DTOs;
using ControleGastos.Api.Enums;
using ControleGastos.Api.Exceptions;
using ControleGastos.Api.Models;
using ControleGastos.Api.Repositories;
using Microsoft.EntityFrameworkCore;
namespace ControleGastos.Api.Services;

public class PessoaService(IPessoaRepository repo)
{
    public async Task<PessoaDto> CriarAsync(CriarPessoaDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome)) throw new AppException("O nome é obrigatório.");
        if (dto.Idade < 0) throw new AppException("A idade deve ser maior ou igual a zero.");
        var p = await repo.CriarAsync(new Pessoa { Nome = dto.Nome.Trim(), Idade = dto.Idade }, ct);
        return new(p.Id, p.Nome, p.Idade);
    }
    public async Task<IReadOnlyList<PessoaDto>> ListarAsync(CancellationToken ct) => (await repo.ListarAsync(ct)).Select(p => new PessoaDto(p.Id, p.Nome, p.Idade)).ToList();
    public async Task ExcluirAsync(int id, CancellationToken ct) { var p = await repo.ObterAsync(id, ct) ?? throw new AppException("Pessoa não encontrada.", 404); await repo.ExcluirAsync(p, ct); }
}
public class TransacaoService(IPessoaRepository pessoas, ITransacaoRepository transacoes)
{
    public async Task<TransacaoDto> CriarAsync(CriarTransacaoDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Descricao)) throw new AppException("A descrição é obrigatória.");
        if (dto.Valor <= 0) throw new AppException("O valor deve ser maior que zero.");
        if (!Enum.IsDefined(dto.Tipo)) throw new AppException("Tipo de transação inválido.");
        if (!Enum.IsDefined(dto.Categoria)) throw new AppException("Categoria inválida.");
        var data = dto.Data?.ToUniversalTime() ?? DateTime.UtcNow;
        if (data > DateTime.UtcNow.AddMinutes(5)) throw new AppException("A data da transação não pode estar no futuro.");
        if (data < DateTime.UtcNow.AddYears(-100)) throw new AppException("A data da transação está fora do período permitido.");
        var pessoa = await pessoas.ObterAsync(dto.PessoaId, ct) ?? throw new AppException("Pessoa não encontrada.", 404);
        // Regra de domínio protegida no servidor, mesmo que a interface também a antecipe.
        if (pessoa.Idade < 18 && dto.Tipo == TipoTransacao.Receita) throw new AppException("Pessoas menores de 18 anos só podem possuir transações do tipo despesa.");
        var t = await transacoes.CriarAsync(new Transacao { Descricao = dto.Descricao.Trim(), Valor = dto.Valor, Tipo = dto.Tipo, PessoaId = pessoa.Id, Categoria = dto.Categoria, Data = data }, ct);
        return new(t.Id, t.Descricao, t.Valor, t.Tipo, pessoa.Id, pessoa.Nome, t.Categoria, t.Data);
    }
    public async Task<IReadOnlyList<TransacaoDto>> ListarAsync(int? pessoaId, TipoTransacao? tipo, CategoriaTransacao? categoria, DateTime? inicio, DateTime? fim, string? busca, CancellationToken ct) => (await transacoes.ListarAsync(pessoaId, tipo, categoria, inicio, fim, busca, ct)).Select(t => new TransacaoDto(t.Id, t.Descricao, t.Valor, t.Tipo, t.PessoaId, t.Pessoa.Nome, t.Categoria, t.Data)).ToList();
    public async Task ExcluirAsync(int id, CancellationToken ct)
    {
        var transacao = await transacoes.ObterAsync(id, ct) ?? throw new AppException("Transação não encontrada.", 404);
        await transacoes.ExcluirAsync(transacao, ct);
    }
}
public class TotaisService(AppDbContext db)
{
    public async Task<TotaisDto> ObterAsync(CancellationToken ct)
    {
        // SQLite não suporta SUM diretamente sobre decimal. Carregamos apenas as pessoas
        // e suas transações para preservar a precisão monetária e agregamos em memória.
        // O Include também garante que pessoas sem lançamentos permaneçam no resultado.
        var registros = await db.Pessoas.AsNoTracking()
            .Include(p => p.Transacoes)
            .OrderBy(p => p.Nome)
            .ToListAsync(ct);
        var pessoas = registros.Select(p =>
        {
            var receitas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => t.Valor);
            var despesas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => t.Valor);
            return new TotalPessoaDto(p.Id, p.Nome, p.Idade, receitas, despesas, receitas - despesas);
        }).ToList();
        var receitas = pessoas.Sum(p => p.TotalReceitas); var despesas = pessoas.Sum(p => p.TotalDespesas);
        return new(pessoas, receitas, despesas, receitas - despesas);
    }
}
