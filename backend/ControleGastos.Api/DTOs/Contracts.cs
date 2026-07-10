using System.ComponentModel.DataAnnotations;
using ControleGastos.Api.Enums;
namespace ControleGastos.Api.DTOs;

public record CriarPessoaDto([property: Required, MaxLength(150)] string Nome, [property: Range(0, int.MaxValue)] int Idade);
public record PessoaDto(int Id, string Nome, int Idade);
public record CriarTransacaoDto([property: Required, MaxLength(250)] string Descricao, [property: Range(typeof(decimal), "0.01", "79228162514264337593543950335")] decimal Valor, TipoTransacao Tipo, [property: Range(1, int.MaxValue)] int PessoaId);
public record TransacaoDto(int Id, string Descricao, decimal Valor, TipoTransacao Tipo, int PessoaId, string PessoaNome);
public record TotalPessoaDto(int PessoaId, string Nome, int Idade, decimal TotalReceitas, decimal TotalDespesas, decimal Saldo);
public record TotaisDto(IReadOnlyList<TotalPessoaDto> Pessoas, decimal TotalGeralReceitas, decimal TotalGeralDespesas, decimal SaldoGeral);
public record ErroDto(int Status, string Mensagem);
