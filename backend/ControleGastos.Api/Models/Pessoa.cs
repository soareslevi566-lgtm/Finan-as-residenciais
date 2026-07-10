namespace ControleGastos.Api.Models;
public class Pessoa
{
    public int Id { get; set; }
    public required string Nome { get; set; }
    public int Idade { get; set; }
    public ICollection<Transacao> Transacoes { get; set; } = [];
}
