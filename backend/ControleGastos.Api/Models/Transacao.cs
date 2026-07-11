using ControleGastos.Api.Enums;
namespace ControleGastos.Api.Models;
public class Transacao
{
    public int Id { get; set; }
    public required string Descricao { get; set; }
    public decimal Valor { get; set; }
    public TipoTransacao Tipo { get; set; }
    public CategoriaTransacao Categoria { get; set; } = CategoriaTransacao.Outros;
    public DateTime Data { get; set; }
    public int PessoaId { get; set; }
    public Pessoa Pessoa { get; set; } = null!;
}
