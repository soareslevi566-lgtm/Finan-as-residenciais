using System.ComponentModel.DataAnnotations;
using ControleGastos.Api.Enums;
namespace ControleGastos.Api.DTOs;

public record CriarPessoaDto([Required, MaxLength(150)] string Nome, [Range(0, int.MaxValue)] int Idade);
public record PessoaDto(int Id, string Nome, int Idade);
public record CriarTransacaoDto([Required, MaxLength(250)] string Descricao, [Range(0.01, double.MaxValue)] decimal Valor, TipoTransacao Tipo, [Range(1, int.MaxValue)] int PessoaId, CategoriaTransacao Categoria = CategoriaTransacao.Outros, DateTime? Data = null);
public record TransacaoDto(int Id, string Descricao, decimal Valor, TipoTransacao Tipo, int PessoaId, string PessoaNome, CategoriaTransacao Categoria, DateTime Data);
public record TotalPessoaDto(int PessoaId, string Nome, int Idade, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);
public record TotaisDto(IReadOnlyList<TotalPessoaDto> Pessoas, decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoGeral);
public record ErroDto(int Status, string Mensagem);
