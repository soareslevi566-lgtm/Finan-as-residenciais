using ControleGastos.Api.Data; using Microsoft.EntityFrameworkCore.Infrastructure; using Microsoft.EntityFrameworkCore.Migrations;
namespace ControleGastos.Api.Migrations;
[DbContext(typeof(AppDbContext)), Migration("202607100001_InitialCreate")]
public partial class InitialCreate : Migration
{
 protected override void Up(MigrationBuilder m)
 {
  m.CreateTable("Pessoas", t => new { Id=t.Column<int>("INTEGER").Annotation("Sqlite:Autoincrement",true), Nome=t.Column<string>("TEXT",maxLength:150,nullable:false), Idade=t.Column<int>("INTEGER",nullable:false) }, constraints: c=>c.PrimaryKey("PK_Pessoas",x=>x.Id));
  m.CreateTable("Transacoes", t=>new { Id=t.Column<int>("INTEGER").Annotation("Sqlite:Autoincrement",true), Descricao=t.Column<string>("TEXT",maxLength:250,nullable:false), Valor=t.Column<decimal>("TEXT",nullable:false), Tipo=t.Column<int>("INTEGER",nullable:false), PessoaId=t.Column<int>("INTEGER",nullable:false) }, constraints: c=>{c.PrimaryKey("PK_Transacoes",x=>x.Id);c.ForeignKey("FK_Transacoes_Pessoas_PessoaId",x=>x.PessoaId,"Pessoas","Id",onDelete:ReferentialAction.Cascade);});
  m.CreateIndex("IX_Transacoes_PessoaId","Transacoes","PessoaId");
 }
 protected override void Down(MigrationBuilder m){m.DropTable("Transacoes");m.DropTable("Pessoas");}
}
