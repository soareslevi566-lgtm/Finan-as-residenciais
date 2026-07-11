using ControleGastos.Api.Data; using ControleGastos.Api.DTOs; using ControleGastos.Api.Enums; using ControleGastos.Api.Exceptions; using ControleGastos.Api.Repositories; using ControleGastos.Api.Services; using Microsoft.Data.Sqlite; using Microsoft.EntityFrameworkCore;
using Xunit;
namespace ControleGastos.Tests;
public class ServiceTests : IDisposable
{
 readonly SqliteConnection cn=new("DataSource=:memory:"); readonly AppDbContext db; readonly PessoaService pessoas; readonly TransacaoService transacoes; readonly TotaisService totais;
 public ServiceTests(){cn.Open();db=new(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(cn).Options);db.Database.EnsureCreated();var pr=new PessoaRepository(db);pessoas=new(pr);transacoes=new(pr,new TransacaoRepository(db));totais=new(db);}
 public void Dispose(){db.Dispose();cn.Dispose();}
 async Task<PessoaDto> Pessoa(string nome="Ana",int idade=25)=>await pessoas.CriarAsync(new(nome,idade),default);
 [Fact] public async Task Cria_pessoa_valida()=>Assert.Equal("Ana",(await Pessoa()).Nome);
 [Fact] public async Task Impede_idade_negativa()=>await Assert.ThrowsAsync<AppException>(()=>pessoas.CriarAsync(new("Ana",-1),default));
 [Fact] public async Task Cria_despesa_para_menor(){var p=await Pessoa(idade:17);Assert.Equal(TipoTransacao.Despesa,(await transacoes.CriarAsync(new("Livro",50,TipoTransacao.Despesa,p.Id),default)).Tipo);}
 [Fact] public async Task Impede_receita_para_menor(){var p=await Pessoa(idade:17);await Assert.ThrowsAsync<AppException>(()=>transacoes.CriarAsync(new("Mesada",50,TipoTransacao.Receita,p.Id),default));}
 [Fact] public async Task Permite_receita_a_partir_de_18(){var p=await Pessoa(idade:18);Assert.Equal(TipoTransacao.Receita,(await transacoes.CriarAsync(new("Salário",100,TipoTransacao.Receita,p.Id),default)).Tipo);}
 [Fact] public async Task Impede_pessoa_inexistente()=>await Assert.ThrowsAsync<AppException>(()=>transacoes.CriarAsync(new("X",10,TipoTransacao.Despesa,999),default));
 [Theory,InlineData(0),InlineData(-1)] public async Task Impede_valor_nao_positivo(decimal v){var p=await Pessoa();await Assert.ThrowsAsync<AppException>(()=>transacoes.CriarAsync(new("X",v,TipoTransacao.Despesa,p.Id),default));}
 [Fact] public async Task Exclusao_em_cascata(){var p=await Pessoa();await transacoes.CriarAsync(new("X",10,TipoTransacao.Despesa,p.Id),default);await pessoas.ExcluirAsync(p.Id,default);Assert.Empty(await transacoes.ListarAsync(null,null,null,null,null,null,default));}
 [Fact] public async Task Calcula_totais_por_pessoa_e_gerais(){var p=await Pessoa();await transacoes.CriarAsync(new("R",300,TipoTransacao.Receita,p.Id),default);await transacoes.CriarAsync(new("D",120,TipoTransacao.Despesa,p.Id),default);var t=await totais.ObterAsync(default);Assert.Equal(180,t.Pessoas[0].Saldo);Assert.Equal(300,t.TotalGeralReceitas);Assert.Equal(120,t.TotalGeralDespesas);Assert.Equal(180,t.SaldoGeral);}
 [Fact] public async Task Pessoa_sem_transacoes_tem_zeros(){await Pessoa();var p=(await totais.ObterAsync(default)).Pessoas[0];Assert.Equal(0,p.TotalReceitas);Assert.Equal(0,p.TotalDespesas);Assert.Equal(0,p.Saldo);}
 [Fact] public async Task Impede_transacao_com_data_futura(){var p=await Pessoa();await Assert.ThrowsAsync<AppException>(()=>transacoes.CriarAsync(new("Futura",10,TipoTransacao.Despesa,p.Id,CategoriaTransacao.Outros,DateTime.UtcNow.AddDays(1)),default));}
 [Fact] public async Task Persiste_categoria_e_data(){var p=await Pessoa();var data=DateTime.UtcNow.AddDays(-2);var t=await transacoes.CriarAsync(new("Mercado",80,TipoTransacao.Despesa,p.Id,CategoriaTransacao.Alimentacao,data),default);Assert.Equal(CategoriaTransacao.Alimentacao,t.Categoria);Assert.Equal(data,t.Data);}
 [Fact] public async Task Filtra_transacoes_por_categoria_e_periodo(){var p=await Pessoa();await transacoes.CriarAsync(new("Ônibus",20,TipoTransacao.Despesa,p.Id,CategoriaTransacao.Transporte,DateTime.UtcNow.AddDays(-1)),default);await transacoes.CriarAsync(new("Cinema",40,TipoTransacao.Despesa,p.Id,CategoriaTransacao.Lazer,DateTime.UtcNow.AddDays(-10)),default);var lista=await transacoes.ListarAsync(p.Id,null,CategoriaTransacao.Transporte,DateTime.UtcNow.AddDays(-2),DateTime.UtcNow,null,default);Assert.Single(lista);Assert.Equal("Ônibus",lista[0].Descricao);}
}
